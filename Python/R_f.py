from Lib.RLCFrequencySweptMeasurement import RLCFrequencySweptMeasurement
from Lib.ScriptShell import ScriptShell
from Drivers.KeysightE4980AL import E4980ALMeasFunction


shell = ScriptShell(title="Resistance_freq")
meas_rf = RLCFrequencySweptMeasurement(shell, E4980ALMeasFunction.R, "Resistance, Ohms")

meas_rf.run()
