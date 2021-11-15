import numpy as np
import argparse
import os
from os import path
from datetime import datetime
import pandas as pd

from Lib import Logger
from Drivers.KeysightE4980AL import *


class ScriptShell:
    @staticmethod
    def _generate_sweep_sequence(start, end, step):
        return np.append(np.arange(start, end, step), end)

    def _parse_args(self):
        p = argparse.ArgumentParser()
        p.add_argument('-from', action='store', required=True)
        p.add_argument('-to', action='store', required=True)
        p.add_argument('-step', action='store', required=True)
        p.add_argument('-V', action='store', required=True)
        p.add_argument('-S', action='store', required=True)
        p.add_argument('-D', action='store', required=True)
        p.add_argument('-ID', action='store', required=True)

        args, unknown = p.parse_known_args()
        args = vars(args)

        self.sweep_seq = self._generate_sweep_sequence(float(args['from']), float(args['to']), float(args['step']))

        self.voltage = float(args['V'])
        self.sample_name = self._preprocess_string_for_filename(args['S'])
        self.device_id = args['ID']

    def _init_device(self):
        meter = KeysightE4980AL(self.device_id)
        meter.set_voltage(self.voltage)

        self.meter = meter

    @staticmethod
    def _preprocess_string_for_filename(s):
        return s.translate(str.maketrans({':': '_', '/': '_', '\\': '_', '*': '_', '?': '_', '"': '_',
                                          '>': '_', '<': '_', '|': '_'}))

    def _get_measurement_id(self):
        return f'{self.sample_name}_{self.title}'

    @staticmethod
    def _get_upper_folder():
        return path.split(os.getcwd())[0]

    def get_save_folder(self):
        if self._save_path is not None:
            return self._save_path
        # get current date only one time
        # to prevent case when part of saved files will have date, for example, 12:00
        # and another part - 12:01

        experiment_date = self.experiment_date

        cd_first_with_date = experiment_date.strftime('%d-%m-%Y') + '_' + self.sample_name
        cd_this_meas = experiment_date.strftime('%H-%M') + '_' + self._get_measurement_id()
        save_path = path.join(self._get_upper_folder(), 'Data', cd_first_with_date, cd_this_meas)

        if not path.isdir(save_path):
            os.makedirs(save_path)

        self._save_path = save_path
        return save_path

    def get_save_file_name(self, ext="dat", preserve_unique=True):
        save_path = self.get_save_folder()

        cd = self.experiment_date.strftime('%d-%m-%Y_%H-%M')
        meas_id = self._get_measurement_id()

        filename = path.join(save_path, f'{cd}_{meas_id}.{ext}')

        # if file, even with this minutes, already exists
        if preserve_unique:
            k = 0
            while os.path.isfile(filename):
                k += 1
                filename = path.join(save_path, f'{cd}_{meas_id}_{k}.{ext}')

        return filename

    def save_data(self, data_dict, preserve_unique=True):
        fname = self.get_save_file_name(preserve_unique=preserve_unique)

        df = pd.DataFrame(data_dict)
        df.to_csv(fname, sep=" ", header=True, index=False, float_format='%.8f')

        print('Data were successfully saved to:', fname)

    def __init__(self, title):
        self.title = title
        self._save_path = None
        self.experiment_date = datetime.now()
        self._parse_args()
        self._init_device()
        self.logger = Logger.Logger(self)
