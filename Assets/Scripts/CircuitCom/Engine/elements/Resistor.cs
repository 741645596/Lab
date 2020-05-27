using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

    /// <summary>
    /// 定值电阻
    /// </summary>
	public class Resistor : CircuitElement {

        protected double _resistance = 100.0;
		/// <summary>
		/// Resistance 电阻值 (ohms)
		/// </summary>
        public double resistance
        {
            get
            {
                return _resistance;
            }
            set
            {
                if (value > 0)
                {
                    _resistance = value;
                }
            }
        }

		public Resistor() : base() 
        {
		}

        public Resistor(double r)
            : base()
        {
            resistance = r;
        }


		public override void calculateCurrent() {
			current = (lead_volt[0] - lead_volt[1]) / resistance;
		}

		public override void stamp(Circuit sim) {
			sim.stampResistor(lead_node[0], lead_node[1], resistance);
		}

	}
}