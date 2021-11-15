from matplotlib import pyplot as plt

from Lib.ScriptShell import ScriptShell
from Drivers.KeysightE4980AL import E4980ALMeasFunction


# must be overridden in child classes
class RLCSweptMeasurement:
    def __init__(self, shell: ScriptShell, function: E4980ALMeasFunction,
                 swept_parameter_title: str, measured_parameter_title: str):
        self.meas_values = []
        self.swept_now = []
        self._saved = False
        self._aborted = False

        self._sweep_seq = shell.sweep_seq
        self.swept_parameter_title = swept_parameter_title
        self.measured_parameter_title = measured_parameter_title

        self.shell = shell
        self.meter = shell.meter
        shell.meter.set_function(function)

    def save_data(self):
        shell = self.shell
        shell.save_data({self.swept_parameter_title: self.swept_now,
                              self.measured_parameter_title: self.meas_values})
        plt.savefig(shell.get_save_file_name(ext='pdf'))
        shell.logger.Save()

    def device_cleanup(self):
        meter = self.meter
        meter.reset_trigger()

    def plot_close_handler(self, unused):
        self._aborted = True
        if not self._saved:
            self.save_data()
            self._saved = True
        self.device_cleanup()

    # must be implemented in a child class
    def set_swept_parameter_value(self, value):
        pass

    def run(self):
        plt.ion()
        fig, ax = plt.subplots(figsize=(14, 8))
        line_main, = ax.plot([], [])

        ax.set_xlabel(self.swept_parameter_title)
        ax.set_ylabel(self.measured_parameter_title)
        ax.grid()
        
        fig.canvas.set_window_title('RLC swept measurement')
        fig.canvas.mpl_connect('close_event', self.plot_close_handler)
        fig.show()

        meter = self.meter

        for sw, data in meter.sweep_and_fetch(self._sweep_seq):
            
            self.swept_now.append(sw)
            self.meas_values.append(data)

            line_main.set_xdata(self.swept_now)
            line_main.set_ydata(self.meas_values)

            ax.relim()
            ax.autoscale_view()
            plt.pause(0.01)
        

        if not self._saved:
            self.save_data()
            
        self.device_cleanup()
        
        plt.ioff()
        plt.show()

