from Drivers import visa_device
from enum import IntEnum


class E4980ALMeasFunction(IntEnum):
    R = 0
    L = 1
    C = 2
    Z = 3


class KeysightE4980AL(visa_device.visa_device):
    def __init__(self, device_id):
        super().__init__(device_id)
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

    def set_function(self, func):
        functions = ['RX', 'LPD', 'CPD', 'ZTD']
        self.SendString(f":FUNCtion:IMPedance:TYPE {functions[func]}")

    def reset_trigger(self):
        self.SendString(':ABORt')
        self.SendString(':TRIGger:SOURce:INTernal')
        
    def sweep_and_fetch(self, frequency_seq):
        # self.SendString("*RST")
        self.SendString("*CLS")
        self.SendString(":ABORt")
        self.SendString("TRIG:SOUR BUS")
        self.SendString("DISP:PAGE LIST")
        self.SendString("FORMat:DATA ASCii")
        self.SendString("LIST:MODE STEP")
        
        frequency_seq_str = [str(i) for i in frequency_seq]
        self.SendString("LIST:FREQ " + ",".join(frequency_seq_str))
        self.SendString("INIT:CONT ON")
        for sw in frequency_seq:
            self.SendString(":TRIG:IMM")
            self.GetString("*SRE?")
            read_data = self.GetString(":FETC?")
            yield sw, float(read_data.split(',')[0])

     