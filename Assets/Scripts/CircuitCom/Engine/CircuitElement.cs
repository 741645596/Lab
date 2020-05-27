using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public abstract class CircuitElement : ICircuitElement 
    {
		public readonly static double pi = 3.14159265358979323846;

        protected Circuit.Lead lead0;
        protected Circuit.Lead lead1;

        public Circuit.Lead leadIn { get { return lead0; } }
        public Circuit.Lead leadOut { get { return lead1; } }

		protected int voltSource = 0;
		protected double current = 0.0d;
		protected int[]    lead_node;
		protected double[] lead_volt;

		public CircuitElement() {
			allocLeads();
		}

        public virtual Circuit.Lead getLead(int n)
        {
            return (n == 0) ? lead0 : (n == 1) ? lead1 : null;
        }

		public virtual int getLeadCount()
        {
            return 2; 
        }

		public virtual double getLeadVoltage(int leadX) {
			return lead_volt[leadX];
		}

        public virtual double getCurrent()
        {
            return current; 
        }
		
		public virtual double getVoltageDelta()
        {
            return lead_volt[0] - lead_volt[1]; 
        }

        public virtual double getPower()
        {
            return getVoltageDelta() * current;
        }
		
		protected void allocLeads() 
        {
            lead0 = new Circuit.Lead(this, 0);
            lead1 = new Circuit.Lead(this, 1);
			lead_node = new int[getLeadCount() + getInternalLeadCount()];
			lead_volt = new double[getLeadCount() + getInternalLeadCount()];
		}

		public virtual int getInternalLeadCount() 
        {
            return 0;
        }

		public virtual void setLeadVoltage(int leadX, double vValue) {
			lead_volt[leadX] = vValue;
			calculateCurrent();
		}

		public virtual void setCurrent(int voltSourceNdx, double c)
        {
            current = c; 
        }

		public virtual void calculateCurrent() { }

        public int getLeadNode(int lead_ndx) {
			return lead_node[lead_ndx];
		}

		public void setLeadNode(int lead_ndx, int node_ndx) {
			lead_node[lead_ndx] = node_ndx;
		}

		public virtual int getVoltageSourceCount()
        {
            return 0; 
        }

        public virtual int getVoltageSource() 
        { 
            return voltSource;
        }
		
		public virtual void setVoltageSource(int leadX, int voltSourceNdx) 
        {
            voltSource = voltSourceNdx; 
        }

		public virtual bool leadsAreConnected(int leadX, int leadY) 
        {
            return true; 
        }
		
		public virtual bool leadIsGround(int leadX)
        { 
            return false;
        }

		public virtual void beginStep(Circuit sim) { }
		public virtual void step(Circuit sim) { }
		public virtual void stamp(Circuit sim) { }

		public virtual void reset() {
			for(int i = 0; i != getLeadCount() + getInternalLeadCount(); i++)
				lead_volt[i] = 0;
		}

		public virtual bool isWire() 
        { 
            return false; 
        }
		
		public virtual bool nonLinear() 
        {
            return false; 
        }

		protected static bool comparePair(int x1, int x2, int y1, int y2) {
			return ((x1 == y1 && x2 == y2) || (x1 == y2 && x2 == y1));
		}
	}
}
