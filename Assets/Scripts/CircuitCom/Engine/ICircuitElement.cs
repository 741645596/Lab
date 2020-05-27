using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpCircuit {

	public class ScopeFrame {
		public double time { get; set; }
		public double current { get; set; }
		public double voltage { get; set; }
	}

	public interface ICircuitElement
    {
        // 电流
        double getCurrent();

        // 电压
        double getVoltageDelta();

        // 接线柱的电压值(电动势)
        double getLeadVoltage(int leadX);

        // 接线柱
        Circuit.Lead getLead(int n);

        // 接线柱数量
        int getLeadCount();

        // 功率
        double getPower();
	}

	public static class ICircuitComponentExtensions {

		public static ScopeFrame GetScopeFrame(this ICircuitElement elem, double time) {
			return new ScopeFrame {
				time = time,
				current = elem.getCurrent(),
				voltage = elem.getVoltageDelta(),
			};
		}

		public static string GetCurrentString(this ICircuitElement elem) {
			return SIUnits.Current(elem.getCurrent());
		}

		public static string GetVoltageString(this ICircuitElement elem) {
			return SIUnits.Voltage(elem.getVoltageDelta());
		}

	}

}
