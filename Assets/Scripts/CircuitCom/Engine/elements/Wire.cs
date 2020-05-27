using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCircuit {

	public class Wire : Resistor {


        private static readonly double defaultResistance = 1E-6;

        public Wire() : base()
        {
            _resistance = defaultResistance;
        }

        //public override void stamp(Circuit sim) {
        //    sim.stampVoltageSource(lead_node[0], lead_node[1], voltSource, 0);
        //}

        //public override int getVoltageSourceCount() {
        //    return 1;
        //}



        //public override bool isWire() {
        //    return true;
        //}

	}
}