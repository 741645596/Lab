using System;
using System.Collections.Generic;
using System.Text;

namespace SharpCircuit
{
    class CircuitNode
    {

        public Circuit.Lead lead;

        public bool Internal { get; set; }


        public List<CircuitNodeLink> links = new List<CircuitNodeLink>();
    }
}
