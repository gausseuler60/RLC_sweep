from Drivers import visa_device
from enum import Enum


class E4980ALMeasFunction(Enum):
    R = 0
    L = 1
    C = 2
    Z = 3


class KeysightE4980AL(visa_device.visa_device):
    def __init__(self):
        super().__init__(self)
        self.SendString(':FORMat:DATA ASCii')
        self.SendString(':ABORt')
        self.SendString(':TRIGger:SOURce:BUS')

    def set_frequency(self, freq: float):
        self.SendString(f':FREQuency:CW {freq}')

    def set_voltage(self, volt: float):
        self.SendString(f':VOLTage:LEVel {volt}')

    def measure(self):
        self.SendString(':TRIGger:IMMediate')
        return float(self.GetString('FETCh?').split(',')[0])

    def set_function(self, func: E4980ALMeasFunction):
        functions = ['RX', 'LPD', 'CPD', 'ZTD']
        self.SendString(f":FUNCtion:IMPedance:TYPE {functions[func]}")

    def reset_trigger(self):
        self.SendString(':ABORt')
        self.SendString(':TRIGger:SOURce:INTernal')
