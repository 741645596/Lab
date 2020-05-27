using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class SwitchSPST : Resistor {

		public Circuit.Lead leadA { get { return lead0; } }
		public Circuit.Lead leadB { get { return lead1; } }

        private static readonly double defaultResistance = 1E-6;

		// position 0 == closed, position 1 == open
        protected int position;

        public bool IsOpen { 
            get
            { 
                return position == 1;
            }
            set 
            {
                position = value ? 1 : 0; 
            }
        }

		protected int posCount;

		public SwitchSPST() : base() {
			position = 0;
			posCount = 2;
            _resistance = defaultResistance;
		}

		public virtual void toggle() {
			position++;
			if(position >= posCount)
				position = 0;
		}

		public virtual void setPosition(int pos) {
			position = pos;
			if(position >= posCount)
				position = 0;
		}

		public override void calculateCurrent() {
            if (position == 1)
            {
                current = 0;
            }
            else
            {
                base.calculateCurrent();
            }
		}

		public override void stamp(Circuit sim) {
			if(position == 0)
            {
                sim.stampResistor(lead_node[0], lead_node[1], _resistance);
            }
                //sim.stampVoltageSource(lead_node[0], lead_node[1], voltSource, 0);
		}

		public override int getVoltageSourceCount() {
            //return (position == 1) ? 0 : 1;
            return 0;
		}

		public override bool leadsAreConnected(int n1, int n2) {
			return position == 0;
		}

		public override bool isWire() {
            return false;
		}

	}
}