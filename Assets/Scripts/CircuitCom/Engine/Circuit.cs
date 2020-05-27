// CirSim.java (c) 2010 by Paul Falstad
// For information about the theory behind this, see Electronic Circuit & System Simulation Methods by Pillage
// http://www.falstad.com/circuit/

using System;
using System.Collections.Generic;

using Snowflake;
using UnityEngine;

namespace SharpCircuit {

	public class Circuit {

        //接线柱
		public class Lead 
        {
            public CircuitElement elem { get; private set; }
			public int ndx { get; private set; }
            public long Id { get; internal set; }
            internal Lead(CircuitElement e, int i)
            { 
                elem = e; 
                ndx = i;
                Id = Circuit.snowflake.NextId();
            }
        }

        //连接（非导线）
        public class Connection : IComparable<Connection>
        {
            public long leadA;
            public long leadB;
            internal Connection(long A, long B)
            {
                leadA = A;
                leadB = B;
            }

            public int CompareTo(Connection obj)
            {
                if (obj == null)
                    return -1;
                if (obj == this)
                    return 0;
                if (this.leadA == obj.leadA && this.leadB == obj.leadB)
                    return 0;
                if (this.leadB == obj.leadA && this.leadA == obj.leadB)
                    return 0;

                return -1;
            }
        }

		public double time { 	get; private set; }
		
		public double timeStep {
			get {
				return _timeStep;
			}
			set {
				_timeStep = value;
				_analyze = true;
			}
		}

		public int speed { get; set; } // 172 // Math.Log(10 * 14.3) * 24 + 61.5;

        //元器件
        public List<CircuitElement> elements = new List<CircuitElement>();

        //电路节点
        List<CircuitNode> nodeList = new List<CircuitNode>();

        //电源
        CircuitElement[] voltageSources;

        public int nodeCount { get { return nodeList.Count; } }

		public bool converged;
		public int subIterations;

        public static IdWorker snowflake = new IdWorker(1, 1);
		
		bool _analyze = true;
		double _timeStep = 5E-6;

		double[][] circuitMatrix; // contains circuit state
		double[] circuitRightSide;
		double[][] origMatrix;
		double[] origRightSide;
		RowInfo[] circuitRowInfo;
		int[] circuitPermute;
		
		int circuitMatrixSize, 
			circuitMatrixFullSize;

		bool circuitNonLinear, 
			 circuitNeedsMap;

        Dictionary<CircuitElement, List<ScopeFrame>> scopeMap = new Dictionary<CircuitElement, List<ScopeFrame>>();

        List<Connection> connectionList = new List<Connection>();

		public Circuit() {
			speed = 172;
		}

        public T Create<T>(params object[] args) where T : CircuitElement
        {
			T circuit = Activator.CreateInstance(typeof(T), args) as T;
			AddElement(circuit);
			return circuit;
		}

        public void AddElement(CircuitElement elm)
        {
			if(!elements.Contains(elm)) {
				elements.Add(elm);
			}
		}

		public Connection Connect(Lead left, Lead right) {
            long id1 = left.Id;
            long id2 = right.Id;
            Connection conn = new Connection(id1, id2);
            connectionList.Add(conn);
            needAnalyze();
            return conn;
		}

        public Connection Connect(CircuitElement left, int leftLeadNdx, CircuitElement right, int rightLeadNdx)
        {
            Lead leftlead = left.getLead(leftLeadNdx);
            Lead rightlead = right.getLead(rightLeadNdx);
            return Connect(leftlead, rightlead);
		}

        public void Disconnect(Lead left, Lead right)
        {
            Connection conn = new Connection(left.Id, right.Id);
            connectionList.Remove(conn);
            needAnalyze();
        }

        public List<ScopeFrame> Watch(CircuitElement component)
        {
			if(!scopeMap.ContainsKey(component)) {
				List<ScopeFrame> scope = new List<ScopeFrame>();
				scopeMap.Add(component, scope);
				return scope;
			} else {
				return scopeMap[component];
			}
		}

		public void resetTime() {
			time = 0;
		}

		public void needAnalyze() {
			_analyze = true;
		}

		public void analyze() 
        {			
			if(elements.Count == 0)
				return;

			nodeList.Clear();

			#region 查找电源和接地元素
			// Search the circuit for a Ground, or Voltage sourcre
            CircuitElement voltageElm = null;
			bool gotGround = false;
			bool gotRail = false;
			for(int i = 0; i != elements.Count; i++)
			{
                CircuitElement elem = elements[i];
				if(elem is Ground) {
					gotGround = true;
					break;
				}

				if(elem is VoltageInput) //RailElm in Java
					gotRail = true;

				if(voltageElm == null && elem is Voltage)
					voltageElm = elem;
			}

            // If no ground and no rails, then the voltage elm's first terminal is ground.
            // 没有电源(单)，没有接地，但是有电源(双)，那么电源的零点为接地点
            if (!gotGround && !gotRail && voltageElm != null)
            {
                CircuitNode cn = new CircuitNode();
                cn.lead = voltageElm.getLead(0);
                nodeList.Add(cn);
            }
            else
            {
                // If the circuit contains a ground, rail, or voltage
                // element, push a temporary node to the node list.
                CircuitNode cn = new CircuitNode();
                cn.lead = null; //Bin: 特殊节点，零点
                nodeList.Add(cn);
            }
			#endregion

			// At this point, there is 1 node in the list, the special `global ground` node.

			#region 电路节点和电源节点
			int vscount = 0; // Number of voltage sources
			for(int i = 0; i != elements.Count; i++) 
			{
                CircuitElement ce = elements[i];
				int leads = ce.getLeadCount();

                for (int j = 0; j != leads; j++)
                {
                    Lead lead = ce.getLead(j);
                    int k;
                    for (k = 0; k != nodeList.Count; k++) //查找现有的每个节点，判断是否相连
                    {
                        //原Java版，这里用元器件的端点的坐标（XY）判断是否相连
                        //我们这里没有坐标，因此连接关系由Lead自己维护
                        CircuitNode cn = nodeList[k];
                        if (cn.lead != null)
                        {
                            FindConnection fc = new FindConnection(this, lead.Id);
                            if (fc.IsConnected(cn.lead.Id))
                                break;
                        }
                    }
                    if (k == nodeList.Count)  //没有找到现有的节点
                    {
                        CircuitNode cn = new CircuitNode();
                        cn.lead = lead;
                        CircuitNodeLink cnl = new CircuitNodeLink();
                        cnl.lead_ndx = j;
                        cnl.element = ce;
                        cn.links.Add(cnl);
                        ce.setLeadNode(j, nodeList.Count);
                        nodeList.Add(cn); //这两行顺序不能颠倒
                    }
                    else
                    {
                        CircuitNodeLink cnl = new CircuitNodeLink();
                        cnl.lead_ndx = j;
                        cnl.element = ce;
                        nodeList[k].links.Add(cnl);
                        ce.setLeadNode(j, k);
                        // if it's the ground node, make sure the node voltage is 0,
                        // cause it may not get set later
                        if (k == 0)
                            ce.setLeadVoltage(j, 0);
                    }
                }
               
				// Push an internal node onto the list for
				// each internal lead on the element.
				int internalLeads = ce.getInternalLeadCount();
				for(int j = 0; j != internalLeads; j++)
                {
                    CircuitNode cn = new CircuitNode();
                    cn.lead = null;
                    cn.Internal = true;
                    CircuitNodeLink cnl = new CircuitNodeLink();
                    cnl.lead_ndx = j + leads;
                    cnl.element = ce;
                    cn.links.Add(cnl);
                    ce.setLeadNode(cnl.lead_ndx, nodeList.Count);
                    nodeList.Add(cn); //这两行顺序不能颠倒
                }
				vscount += ce.getVoltageSourceCount();
			}
			#endregion

			// 创建电源节点数组
			// 同时判断电路是否线性

			// VoltageSourceId -> CircuitElement map
            voltageSources = new CircuitElement[vscount];
			vscount = 0;
			circuitNonLinear = false;


            for (int i = 0; i != elements.Count; i++)
            {
                CircuitElement ce = elements[i];
                if (ce.nonLinear())
                    circuitNonLinear = true;

                // Assign each votage source in the element a globally unique id,
                // (the index of the next open slot in voltageSources)
                for (int j = 0; j != ce.getVoltageSourceCount(); j++)
                {
                    voltageSources[vscount] = ce;
                    ce.setVoltageSource(j, vscount);
                    vscount++;
                }
            }

			#region 创建矩阵
			int matrixSize = nodeList.Count - 1 + vscount;

			// setup circuitMatrix
			circuitMatrix = new double[matrixSize][];
			for(int z = 0; z < matrixSize; z++)
				circuitMatrix[z] = new double[matrixSize];

			circuitRightSide = new double[matrixSize];

			// setup origMatrix
			origMatrix = new double[matrixSize][];
			for(int z = 0; z < matrixSize; z++)
				origMatrix[z] = new double[matrixSize];

			origRightSide = new double[matrixSize];

			// setup circuitRowInfo
			circuitRowInfo = new RowInfo[matrixSize];
			for(int i = 0; i != matrixSize; i++)
				circuitRowInfo[i] = new RowInfo();

			circuitPermute = new int[matrixSize];
			circuitMatrixSize = circuitMatrixFullSize = matrixSize;
			circuitNeedsMap = false;
			#endregion

			// Stamp linear circuit elements.
			for(int i = 0; i != elements.Count; i++)
				elements[i].stamp(this);

			#region 查找连接不正确的节点
			bool[] closure = new bool[nodeList.Count];
			bool changed = true;
			closure[0] = true;
			while(changed)
            {
				changed = false;
				for(int i = 0; i != elements.Count; i++)
                {
                    CircuitElement ce = elements[i];
					// loop through all ce's nodes to see if they are connected
					// to other nodes not in closure
					for(int leadX = 0; leadX < ce.getLeadCount(); leadX++)
                    {
						if(!closure[ce.getLeadNode(leadX)])
                        {
							if(ce.leadIsGround(leadX))
								closure[ce.getLeadNode(leadX)] = changed = true;
							continue;
						}
						for(int k = 0; k != ce.getLeadCount(); k++) {
							if(leadX == k)
                                continue;
							int kn = ce.getLeadNode(k);
							if(ce.leadsAreConnected(leadX, k) && !closure[kn])
                            {
								closure[kn] = true;
								changed = true;
							}
						}
					}
				}

				if(changed)
					continue;

				// connect unconnected nodes
				for(int i = 0; i != nodeList.Count; i++)
                {
					if(!closure[i] && !nodeList[i].Internal)
                    {
						//System.out.println("node " + i + " unconnected");
						stampResistor(0, i, 1E8);
						closure[i] = true;
						changed = true;
						break;
					}
				}
			}
			#endregion

			#region  电路正确性检查
			for(int i = 0; i != elements.Count; i++) 
			{
                CircuitElement ce = elements[i];

				// look for inductors with no current path
				if(ce is InductorElm) {
					FindPathInfo fpi = new FindPathInfo(this, FindPathInfo.PathType.INDUCT, ce, ce.getLeadNode(1));
					// first try findPath with maximum depth of 5, to avoid slowdowns
					if(!fpi.findPath(ce.getLeadNode(0), 5) && !fpi.findPath(ce.getLeadNode(0))) {
						//System.out.println(ce + " no path");
						ce.reset();
					}
				}

				// look for current sources with no current path
				if(ce is CurrentSource) {
					FindPathInfo fpi = new FindPathInfo(this, FindPathInfo.PathType.INDUCT, ce, ce.getLeadNode(1));
					if(!fpi.findPath(ce.getLeadNode(0)))
						panic("No path for current source!", ce);
				}

				// look for voltage source loops
				if((ce is Voltage && ce.getLeadCount() == 2) || ce is Wire) {
					FindPathInfo fpi = new FindPathInfo(this, FindPathInfo.PathType.VOLTAGE, ce, ce.getLeadNode(1));
					if(fpi.findPath(ce.getLeadNode(0)))
						panic("Voltage source/wire loop with no resistance!", ce);
				}

				// look for shorted caps, or caps w/ voltage but no R
				if(ce is CapacitorElm) {
					FindPathInfo fpi = new FindPathInfo(this, FindPathInfo.PathType.SHORT, ce, ce.getLeadNode(1));
					if(fpi.findPath(ce.getLeadNode(0))) {
						//System.out.println(ce + " shorted");
						ce.reset();
					} else {
						fpi = new FindPathInfo(this, FindPathInfo.PathType.CAP_V, ce, ce.getLeadNode(1));
						if(fpi.findPath(ce.getLeadNode(0)))
							panic("Capacitor loop with no resistance!", ce);
					}
				}
			}
			#endregion

			#region 简化矩阵 这块会导致电池无法并联，先去掉
            //for(int i = 0; i != matrixSize; i++) {
            //    int qm = -1, qp = -1;
            //    double qv = 0;
            //    RowInfo re = circuitRowInfo[i];

            //    if(re.lsChanges || re.dropRow || re.rsChanges)
            //        continue;

            //    double rsadd = 0;

            //    // look for rows that can be removed
            //    int leadX = 0;
            //    for(; leadX != matrixSize; leadX++) {

            //        double q = circuitMatrix[i][leadX];
            //        if(circuitRowInfo[leadX].type == RowInfo.ROW_CONST) {
            //            // keep a running total of const values that have been removed already
            //            rsadd -= circuitRowInfo[leadX].value * q;
            //            continue;
            //        }

            //        if(q == 0)
            //            continue;

            //        if(qp == -1) {
            //            qp = leadX;
            //            qv = q;
            //            continue;
            //        }

            //        if(qm == -1 && q == -qv) {
            //            qm = leadX;
            //            continue;
            //        }

            //        break;
            //    }

            //    if(leadX == matrixSize) {

            //        if(qp == -1)
            //            panic("Matrix error", null);

            //        RowInfo elt = circuitRowInfo[qp];
            //        if(qm == -1) {
            //            // we found a row with only one nonzero entry;
            //            // that value is a constant
            //            for(int k = 0; elt.type == RowInfo.ROW_EQUAL && k < 100; k++) {
            //                // follow the chain
            //                // System.out.println("following equal chain from " + i + " " + qp + " to " + elt.nodeEq);
            //                qp = elt.nodeEq;
            //                elt = circuitRowInfo[qp];
            //            }

            //            if(elt.type == RowInfo.ROW_EQUAL) {
            //                // break equal chains
            //                // System.out.println("Break equal chain");
            //                elt.type = RowInfo.ROW_NORMAL;
            //                continue;
            //            }

            //            if(elt.type != RowInfo.ROW_NORMAL) {
            //                //System.out.println("type already " + elt.type + " for " + qp + "!");
            //                continue;
            //            }

            //            elt.type = RowInfo.ROW_CONST;
            //            elt.value = (circuitRightSide[i] + rsadd) / qv;
            //            circuitRowInfo[i].dropRow = true;
            //            // System.out.println(qp + " * " + qv + " = const " + elt.value);
            //            i = -1; // start over from scratch
            //        } else if(circuitRightSide[i] + rsadd == 0) {
            //            // we found a row with only two nonzero entries, and one
            //            // is the negative of the other; the values are equal
            //            if(elt.type != RowInfo.ROW_NORMAL) {
            //                // System.out.println("swapping");
            //                int qq = qm;
            //                qm = qp;
            //                qp = qq;
            //                elt = circuitRowInfo[qp];
            //                if(elt.type != RowInfo.ROW_NORMAL) {
            //                    // we should follow the chain here, but this hardly
            //                    // ever happens so it's not worth worrying about
            //                    //System.out.println("swap failed");
            //                    continue;
            //                }
            //            }
            //            elt.type = RowInfo.ROW_EQUAL;
            //            elt.nodeEq = qm;
            //            circuitRowInfo[i].dropRow = true;
            //            // System.out.println(qp + " = " + qm);
            //        }
            //    }
            //}

			// == Find size of new matrix
			int nn = 0;
			for(int i = 0; i != matrixSize; i++) {
				RowInfo elt = circuitRowInfo[i];
				if(elt.type == RowInfo.ROW_NORMAL) {
					elt.mapCol = nn++;
					// System.out.println("col " + i + " maps to " + elt.mapCol);
					continue;
				}
				if(elt.type == RowInfo.ROW_EQUAL) {
					RowInfo e2 = null;
					// resolve chains of equality; 100 max steps to avoid loops
					for(int leadX = 0; leadX != 100; leadX++) {
						e2 = circuitRowInfo[elt.nodeEq];
						if(e2.type != RowInfo.ROW_EQUAL)
							break;

						if(i == e2.nodeEq)
							break;

						elt.nodeEq = e2.nodeEq;
					}
				}
				if(elt.type == RowInfo.ROW_CONST)
					elt.mapCol = -1;
			}

			for(int i = 0; i != matrixSize; i++) {
				RowInfo elt = circuitRowInfo[i];
				if(elt.type == RowInfo.ROW_EQUAL) {
					RowInfo e2 = circuitRowInfo[elt.nodeEq];
					if(e2.type == RowInfo.ROW_CONST) {
						// if something is equal to a const, it's a const
						elt.type = e2.type;
						elt.value = e2.value;
						elt.mapCol = -1;
					} else {
						elt.mapCol = e2.mapCol;
					}
				}
			}

			// == Make the new, simplified matrix.
			int newsize = nn;
			double[][] newmatx = new double[newsize][];
			for(int z = 0; z < newsize; z++)
				newmatx[z] = new double[newsize];

			double[] newrs = new double[newsize];
			int ii = 0;
			for(int i = 0; i != matrixSize; i++) {
				RowInfo rri = circuitRowInfo[i];
				if(rri.dropRow) {
					rri.mapRow = -1;
					continue;
				}
				newrs[ii] = circuitRightSide[i];
				rri.mapRow = ii;
				// System.out.println("Row " + i + " maps to " + ii);
				for(int leadX = 0; leadX != matrixSize; leadX++) {
					RowInfo ri = circuitRowInfo[leadX];
					if(ri.type == RowInfo.ROW_CONST) {
						newrs[ii] -= ri.value * circuitMatrix[i][leadX];
					} else {
						newmatx[ii][ri.mapCol] += circuitMatrix[i][leadX];
					}
				}
				ii++;
			}
			#endregion

			#region //// Copy matrix to orig ////
			circuitMatrix = newmatx;
			circuitRightSide = newrs;
			matrixSize = circuitMatrixSize = newsize;

			// copy `rightSide` to `origRightSide`
			for(int i = 0; i != matrixSize; i++)
				origRightSide[i] = circuitRightSide[i];

			// copy `matrix` to `origMatrix`
			for(int i = 0; i != matrixSize; i++)
				for(int leadX = 0; leadX != matrixSize; leadX++)
					origMatrix[i][leadX] = circuitMatrix[i][leadX];
			#endregion

			circuitNeedsMap = true;
			_analyze = false;

			// If the matrix is linear, we can do the lu_factor 
			// here instead of needing to do it every frame.
			if(!circuitNonLinear)
				if(!lu_factor(circuitMatrix, circuitMatrixSize, circuitPermute))
					panic("Singular matrix!", null);
		}


        public void NdClearElements()
        {
            elements.Clear();
            scopeMap.Clear();
            nodeList.Clear();
            connectionList.Clear();
        }


        public bool NDDoticks()
        {
            try
            {
                if (elements.Count == 0) return false;
                analyze();
                if (_analyze) return false;
                tick();
                if (circuitMatrix == null) return false;
                foreach (KeyValuePair<CircuitElement, List<ScopeFrame>> kvp in scopeMap)
                    kvp.Value.Add(kvp.Key.GetScopeFrame(time));
                return true;
            }
            catch (CircuitException e)
            {
                //EngineBrokenWnd wnd = WndManager.GetWnd<EngineBrokenWnd>();
                //if (wnd != null)
                //{
                //    wnd.ShowTipInfo();
                //}
                return false;
            }

        }


		public bool doTick() {
            try
            {
                return doTicks(1);
            }
            catch (CircuitException e)
            {
                //EngineBrokenWnd wnd = WndManager.GetWnd<EngineBrokenWnd>();
                //if (wnd != null)
                //{
                //    wnd.ShowTipInfo();
                //}
                return false;
            }
		}

		public bool doTicks(int ticks) {
			if(elements.Count == 0) 
                return true;
			if(_analyze) 
                analyze();
			if(_analyze)
                return false;
			for(int x = 0; x < ticks; x++ )
			{
				tick();
				if(circuitMatrix == null) 
                    return false;
                foreach (KeyValuePair<CircuitElement, List<ScopeFrame>> kvp in scopeMap)
					kvp.Value.Add(kvp.Key.GetScopeFrame(time));
			}
            return true;			
		}

		bool tick() {

            if (elements.Count == 0)
            {
                circuitMatrix = null;
                return true;
            }

            // Execute beginStep() on all elements
            for (int i = 0; i != elements.Count; i++)
				elements[i].beginStep(this);

			int subiter;
			int subiterCount = 5000;
			for(subiter = 0; subiter != subiterCount; subiter++) {
				converged = true;
				subIterations = subiter;

				// Copy `origRightSide` to `circuitRightSide`
				for(int i = 0; i != circuitMatrixSize; i++)
					circuitRightSide[i] = origRightSide[i];

				// If the circuit is non linear, copy
				// `origMatrix` to `circuitMatrix`
				if(circuitNonLinear)
					for(int i = 0; i != circuitMatrixSize; i++)
						for(int j = 0; j != circuitMatrixSize; j++)
							circuitMatrix[i][j] = origMatrix[i][j];

				// Execute step() on all elements
				for(int i = 0; i != elements.Count; i++)
					elements[i].step(this);
				
				// Can't have any values in the matrix be NaN or Inf
				for(int j = 0; j != circuitMatrixSize; j++) {
					for(int i = 0; i != circuitMatrixSize; i++) {
						double x = circuitMatrix[i][j];
                        if (Double.IsNaN(x) || Double.IsInfinity(x))
                        {
                            panic("NaN/Infinite matrix!", null);
                            return false;
                        }
					}
				}
				
				// If the circuit is non-Linear, factor it now,
				// if it's linear, it was factored in analyze()
				if(circuitNonLinear)
                {
					// Break if the circuit has converged.
					if(converged && subiter > 0)
                        break;
					if(!lu_factor(circuitMatrix, circuitMatrixSize, circuitPermute))
                    {
                        panic("Singular matrix!", null);
                        return false;
                    }
				}

				// Solve the factorized matrix
				lu_solve(circuitMatrix, circuitMatrixSize, circuitPermute, circuitRightSide);

				for(int j = 0; j != circuitMatrixFullSize; j++)
                {
					double res = 0;
					RowInfo ri = circuitRowInfo[j];
					res = (ri.type == RowInfo.ROW_CONST) ? ri.value : circuitRightSide[ri.mapCol];

					// If any resuit is NaN, break
					if(Double.IsNaN(res)) {
						converged = false;
						break;
					}

					if(j < nodeList.Count - 1)
                    {
                        CircuitNode cn = nodeList[j + 1];
                        for (int k = 0; k != cn.links.Count; k++)
                        {
                            CircuitNodeLink cnl = cn.links[k];
                            cnl.element.setLeadVoltage(cnl.lead_ndx, res);
                        }
                    }
                    else
                    {
						int ji = j - (nodeList.Count - 1);
						voltageSources[ji].setCurrent(ji, res);
					}
				}

				// if the matrix is linear, we don't
				// need to do any more iterations
				if(!circuitNonLinear)
					break;
			}

            //if(subiter > 5)
            //    Debug.LogF("Nonlinear curcuit converged after {0} iterations.", subiter);
			
			if(subiter == subiterCount)
            {
                panic("Convergence failed!", null);
                return false;
            }

			time = Math.Round(time + timeStep, 12); // Round to 12 digits

            return true;
		}

        public void panic(String why, CircuitElement elem)
        {
			circuitMatrix = null;
			_analyze = true;
            Debug.Log(why);
			throw new CircuitException(why, elem);
		}

        #region //// Stamp ////

        // http://en.wikipedia.org/wiki/Electrical_element

        public void updateVoltageSource(int n1, int n2, int vs, double v) {
			int vn = nodeList.Count + vs;
			stampRightSide(vn, v);
		}

		public void stampCurrentSource(int n1, int n2, double i) {
			stampRightSide(n1, -i);
			stampRightSide(n2, i);
		}

		// stamp independent voltage source #vs, from n1 to n2, amount v
		public void stampVoltageSource(int n1, int n2, int vs, double v) {
			int vn = nodeList.Count + vs;
			stampMatrix(vn, n1, -1);
			stampMatrix(vn, n2, 1);
			stampRightSide(vn, v);
			stampMatrix(n1, vn, 1);
			stampMatrix(n2, vn, -1);
		}

		// use this if the amount of voltage is going to be updated in doStep()
		public void stampVoltageSource(int n1, int n2, int vs) {
			int vn = nodeList.Count + vs;
			stampMatrix(vn, n1, -1);
			stampMatrix(vn, n2, 1);
			stampRightSide(vn);
			stampMatrix(n1, vn, 1);
			stampMatrix(n2, vn, -1);
		}

		public void stampResistor(int n1, int n2, double r) {
			double r0 = 1 / r;
			stampMatrix(n1, n1, r0);
			stampMatrix(n2, n2, r0);
			stampMatrix(n1, n2, -r0);
			stampMatrix(n2, n1, -r0);
		}

		public void stampConductance(int n1, int n2, double r0) {
			stampMatrix(n1, n1, r0);
			stampMatrix(n2, n2, r0);
			stampMatrix(n1, n2, -r0);
			stampMatrix(n2, n1, -r0);
		}

		/// <summary>
		/// Voltage-controlled voltage source.
		/// Control voltage source vs with voltage from n1 to n2 
		/// (must also call stampVoltageSource())
		/// </summary>
		public void stampVCVS(int n1, int n2, double coef, int vs) {
			int vn = nodeList.Count + vs;
			stampMatrix(vn, n1, coef);
			stampMatrix(vn, n2, -coef);
		}

		/// <summary>
		/// Voltage-controlled current source.
		/// Current from cn1 to cn2 is equal to voltage from vn1 to vn2, divided by g 
		/// </summary>
		public void stampVCCS(int cn1, int cn2, int vn1, int vn2, double g) {
			stampMatrix(cn1, vn1, g);
			stampMatrix(cn2, vn2, g);
			stampMatrix(cn1, vn2, -g);
			stampMatrix(cn2, vn1, -g);
		}

		// Current-controlled voltage source (CCVS)?

		/// <summary>
		/// Current-controlled current source.
		/// Stamp a current source from n1 to n2 depending on current through vs 
		/// </summary>
		public void stampCCCS(int n1, int n2, int vs, double gain) {
			int vn = nodeList.Count + vs;
			stampMatrix(n1, vn, gain);
			stampMatrix(n2, vn, -gain);
		}

		// stamp value x in row i, column j, meaning that a voltage change
		// of dv in node j will increase the current into node i by x dv
		// (Unless i or j is a voltage source node.)
		public void stampMatrix(int i, int j, double x) {
			if(i > 0 && j > 0) {
				if(circuitNeedsMap) {
					i = circuitRowInfo[i - 1].mapRow;
					RowInfo ri = circuitRowInfo[j - 1];
					if(ri.type == RowInfo.ROW_CONST) {
						circuitRightSide[i] -= x * ri.value;
						return;
					}
					j = ri.mapCol;
				} else {
					i--;
					j--;
				}
				circuitMatrix[i][j] += x;
			}
		}

		// stamp value x on the right side of row i, representing an
		// independent current source flowing into node i
		public void stampRightSide(int i, double x) {
			if(i > 0) {
				i = (circuitNeedsMap) ? circuitRowInfo[i - 1].mapRow : i - 1;
				circuitRightSide[i] += x;
			}
		}

		// indicate that the value on the right side of row i changes in doStep()
		public void stampRightSide(int i) {
			if(i > 0) circuitRowInfo[i - 1].rsChanges = true;
		}

		// indicate that the values on the left side of row i change in doStep()
		public void stampNonLinear(int i) {
			if(i > 0) circuitRowInfo[i - 1].lsChanges = true;
		}
		#endregion

		// Factors a matrix into upper and lower triangular matrices by
		// gaussian elimination. On entry, a[0..n-1][0..n-1] is the
		// matrix to be factored. ipvt[] returns an integer vector of pivot
		// indices, used in the lu_solve() routine.
		// http://en.wikipedia.org/wiki/Crout_matrix_decomposition
		public static bool lu_factor(double[][] a, int n, int[] ipvt) {
			double[] scaleFactors;
			int i, j, k;

			scaleFactors = new double[n];

			// divide each row by its largest element, keeping track of the
			// scaling factors
			for(i = 0; i != n; i++) {
				double largest = 0;
				for(j = 0; j != n; j++) {
					double x = Math.Abs(a[i][j]);
					if(x > largest)
						largest = x;
				}
				// if all zeros, it's a singular matrix
				if(largest == 0)
					return false;
				scaleFactors[i] = 1.0 / largest;
			}

			// use Crout's method; loop through the columns
			for(j = 0; j != n; j++) {

				// calculate upper triangular elements for this column
				for(i = 0; i != j; i++) {
					double q = a[i][j];
					for(k = 0; k != i; k++)
						q -= a[i][k] * a[k][j];
					a[i][j] = q;
				}

				// calculate lower triangular elements for this column
				double largest = 0;
				int largestRow = -1;
				for(i = j; i != n; i++) {
					double q = a[i][j];
					for(k = 0; k != j; k++)
						q -= a[i][k] * a[k][j];
					a[i][j] = q;
					double x = Math.Abs(q);
					if(x >= largest) {
						largest = x;
						largestRow = i;
					}
				}

				// pivoting
				if(j != largestRow) {
					for(k = 0; k != n; k++) {
						double x = a[largestRow][k];
						a[largestRow][k] = a[j][k];
						a[j][k] = x;
					}
					scaleFactors[largestRow] = scaleFactors[j];
				}

				// keep track of row interchanges
				ipvt[j] = largestRow;

				// avoid zeros
				if(a[j][j] == 0.0)
					a[j][j] = 1e-18;

				if(j != n - 1) {
					double mult = 1.0 / a[j][j];
					for(i = j + 1; i != n; i++)
						a[i][j] *= mult;
				}
			}
			return true;
		}

		// Solves the set of n linear equations using a LU factorization
		// previously performed by lu_factor. On input, b[0..n-1] is the right
		// hand side of the equations, and on output, contains the solution.
		public static void lu_solve(double[][] a, int n, int[] ipvt, double[] b) {
			// find first nonzero b element
			int i;
			for(i = 0; i != n; i++) {
				int row = ipvt[i];
				double swap = b[row];
				b[row] = b[i];
				b[i] = swap;
				if(swap != 0) 
					break;
			}

			int bi = i++;
			for(; i < n; i++) {
				int row = ipvt[i];
				double tot = b[row];

				b[row] = b[i];
				// forward substitution using the lower triangular matrix
				for(int j = bi; j < i; j++)
					tot -= a[i][j] * b[j];

				b[i] = tot;
			}

			for(i = n - 1; i >= 0; i--) {
				double tot = b[i];

				// back-substitution using the upper triangular matrix
				for(int j = i + 1; j != n; j++)
					tot -= a[i][j] * b[j];

				b[i] = tot / a[i][i];
			}
		}

		private class FindPathInfo {

			public enum PathType {
				INDUCT,
				VOLTAGE,
				SHORT,
				CAP_V,
			}

			Circuit sim;
			bool[] used;
			int dest;
            CircuitElement firstElm;
			PathType type;

            public FindPathInfo(Circuit r, PathType t, CircuitElement e, int d)
            {
				sim = r;
				dest = d;
				type = t;
				firstElm = e;
				used = new bool[sim.nodeList.Count];
			}

			public bool findPath(int n1) {
				return findPath(n1, -1);
			}

			public bool findPath(int n1, int depth) {
				if(n1 == dest)
					return true;

				if(depth-- == 0)
					return false;

				if(used[n1])
					return false;

				used[n1] = true;
				for(int i = 0; i != sim.elements.Count; i++) {

                    CircuitElement ce = sim.elements[i];
					if(ce == firstElm)
						continue;

					if(type == PathType.INDUCT)
						if(ce is CurrentSource)
							continue;

					if(type == PathType.VOLTAGE)
						if(!(ce.isWire() || ce is Voltage))
							continue;

					if(type == PathType.SHORT && !ce.isWire())
						continue;

					if(type == PathType.CAP_V)
						if(!(ce.isWire() || ce is CapacitorElm || ce is Voltage))
							continue;

					if(n1 == 0) {
						// look for posts which have a ground connection;
						// our path can go through ground
						for(int z = 0; z != ce.getLeadCount(); z++) {
							if(ce.leadIsGround(z) && findPath(ce.getLeadNode(z), depth)) {
								used[n1] = false;
								return true;
							}
						}
					}

					int j;
					for(j = 0; j != ce.getLeadCount(); j++)
						if(ce.getLeadNode(j) == n1)
							break;

					if(j == ce.getLeadCount())
						continue;

					if(ce.leadIsGround(j) && findPath(0, depth)) {
						// System.out.println(ce + " has ground");
						used[n1] = false;
						return true;
					}

					if(type == PathType.INDUCT && ce is InductorElm) {
						double c = ce.getCurrent();
						if(j == 0)
							c = -c;

						// System.out.println("matching " + c + " to " + firstElm.getCurrent());
						// System.out.println(ce + " " + firstElm);
						if(Math.Abs(c - firstElm.getCurrent()) > 1e-10)
							continue;
					}

					for(int k = 0; k != ce.getLeadCount(); k++) {
						if(j == k)
							continue;

						// System.out.println(ce + " " + ce.getNode(j) + "-" + ce.getNode(k));
						if(ce.leadsAreConnected(j, k) && findPath(ce.getLeadNode(k), depth)) {
							// System.out.println("got findpath " + n1);
							used[n1] = false;
							return true;
						}
						// System.out.println("back on findpath " + n1);
					}
				}

				used[n1] = false;
				// System.out.println(n1 + " failed");
				return false;
			}
		}

		public class CircuitException : System.Exception {

            public CircuitElement element { get; private set; }

            public CircuitException(string why, CircuitElement elem)
                : base(why)
            {
				element = elem;
			}
		}

        class FindConnection
        {
            long destlead;
            Circuit sim;
            Dictionary<long, bool> usedMap = new Dictionary<long, bool>();

            public FindConnection(Circuit s, long dest)
            {
                sim = s;
                destlead = dest;
            }

            public bool IsConnected(long n1)
            {
                if (n1 == destlead)
                    return true;

                if (usedMap.ContainsKey(n1) && usedMap[n1])
                    return false;

                usedMap[n1] = true;
                for (int i = 0; i < sim.connectionList.Count; i++)
                {
                    Connection conn = sim.connectionList[i];
                    if (conn.leadA == n1)
                    {
                        if (IsConnected(conn.leadB))
                            return true;
                    }
                    else if (conn.leadB == n1)
                    {
                        if (IsConnected(conn.leadA))
                            return true;
                    }
                }

                return false;
            }
        }
	}
}