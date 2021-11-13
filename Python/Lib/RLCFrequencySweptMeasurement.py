from Drivers.KeysightE4980AL import E4980ALMeasFunction
from Lib.RLCSweptMeasurement import RLCSweptMeasurement
from Lib.ScriptShell import ScriptShell


class RLCFrequencySweptMeasurement(RLCSweptMeasurement):
    def __init__(self, shell: ScriptShell, function: E4980ALMeasFunction, measured_parameter_title: str):
        super().__init__(shell, function, "Frequency, Hz", measured_parameter_title)

    def set_swept_parameter_value(self, value):
        self.meter.set_frequency(value)
