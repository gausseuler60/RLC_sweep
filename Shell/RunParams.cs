using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;

namespace Shell
{
    class RunParams
    {
        private String strSampleName;
        private String strDeviceID;
        private int iDeviceModel;

        private String[] strScripts = new String[] { "R_f", "L_f", "C_f", "Z_f" };

        private int iCurrentTab = -1;

        private TextBox txtSweepFrom, txtSweepTo, txtSweepStep, txtVoltageLevel;

        private const double dMinFrequency = 20;
        private const double dMaxFrequency = 1e+6;
        private const double eps = 1e-3;

        public void SetSampleName(String strNewName)
        {
            strSampleName = strNewName;
        }

        public void SetDeviceID(String strNewID)
        {
            strDeviceID = strNewID;
        }

        public void SetDeviceModel(int iNewModel)
        {
            iDeviceModel = iNewModel;
        }

        // Input validation functions
        private bool ValidateOneValue(double value, double min_value, double max_value, String value_name, String value_units)
        {
            const String sErrorHeader = "Invalid value";
            bool fRes = true;
            String sErrorMessage = "";

            if (value - eps > max_value)
            {
                sErrorMessage = String.Format("Invalid {0}: {1} {2}, maximum for a device is {3} {2}",
                    value_name, value, value_units, max_value);
                fRes = false;
            }
            if (value + eps < min_value)
            {
                sErrorMessage = String.Format("Invalid {0}: {1} {2}, minimum for a device is {3} {2}",
                    value_name, value, value_units, min_value);
                fRes = false;
            }

            if(!fRes)
                MessageBox.Show(sErrorMessage, sErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return fRes;
        }

        private bool ValidateSweepParameters(double sweep_start, double sweep_end, double sweep_step, String value_units)
        {
            if((sweep_start - eps > sweep_end && sweep_step > 0) || (sweep_end - eps > sweep_start && sweep_step < 0))
            {
                string sErrorMessage = String.Format("Start value of {0} {1} is greater than the end value of {2} {1}",
                    sweep_end, value_units, sweep_start);
                MessageBox.Show(sErrorMessage, "Invalid swept values", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            double sweep_range = sweep_end - sweep_start;
            if(sweep_step - eps > sweep_range)
            {
                string sErrorMessage = String.Format("Sweep step of {0} {1} is greater than sweep range of {2} {1} (from {3} {1} to {4} {1})",
                    sweep_step, value_units, sweep_range, sweep_start, sweep_end);
                MessageBox.Show(sErrorMessage, "Invalid sweep step", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private bool ValidateUserInputValues()
        {
            NumberFormatInfo f = new NumberFormatInfo
            {
                NumberDecimalSeparator = "."
            };
            double dSweepStart = double.Parse(txtSweepFrom.Text, f);
            double dSweepEnd = double.Parse(txtSweepTo.Text, f);
            double dSweepStep = double.Parse(txtSweepStep.Text, f);

            return ValidateOneValue(dSweepStart, dMinFrequency, dMaxFrequency, "frequency", "Hz")
                && ValidateOneValue(dSweepEnd, dMinFrequency, dMaxFrequency, "frequency", "Hz")
                && ValidateSweepParameters(dSweepStart, dSweepEnd, dSweepStep, "Hz");
        }

        // Handler for change value in text fields with decimal values
        // Checks whether an entered value is a number and marks it with a red color if it is not
        private void HandleTextFieldDecimalChange(object sender, EventArgs e)
        {
            InputValidator.HandleTextFieldChange<double>((TextBox)sender);
        }

        // Handler for change value in text fields with decimal values
        // Does not allow to enter non-numeric characters
        private void HandleTextFieldKeyPress(object sender, KeyPressEventArgs e)
        {
            InputValidator.HandleKeyEvent(e, true, true);
        }

        private void LoadTabSettings(int iTab)
        {
            String sScript = strScripts[iTab];
            String strRead;
            Settings sett = new Settings();

            strRead = "20";
            sett.TryLoadSetting(String.Format("{0}_SweepFrom", sScript), ref strRead);
            txtSweepFrom.Text = strRead;

            strRead = "1000";
            sett.TryLoadSetting(String.Format("{0}_SweepTo", sScript), ref strRead);
            txtSweepTo.Text = strRead;

            strRead = "10";
            sett.TryLoadSetting(String.Format("{0}_SweepStep", sScript), ref strRead);
            txtSweepStep.Text = strRead;

            strRead = "1e-6";
            sett.TryLoadSetting(String.Format("{0}_VoltageLevel", sScript), ref strRead);
            txtVoltageLevel.Text = strRead;
        }

        private void SaveTabSettings(int iTab)
        {
            String sScript = strScripts[iTab];

            Settings sett = new Settings();
            sett.SaveSetting(String.Format("{0}_SweepFrom", sScript), txtSweepFrom.Text);
            sett.SaveSetting(String.Format("{0}_SweepTo", sScript), txtSweepTo.Text);
            sett.SaveSetting(String.Format("{0}_SweepStep", sScript), txtSweepStep.Text);
            sett.SaveSetting(String.Format("{0}_VoltageLevel", sScript), txtVoltageLevel.Text);
        }

        // for measurement parameters tab
        public void UpdateCurrentTab(int nNewTab)
        {
            // if not first start, and there was a previous tab
            // and a previous tab is not a settings tab
            if (iCurrentTab != -1 && iCurrentTab != strScripts.Length)
            { 
                SaveTabSettings(iCurrentTab);
                // remove event handlers from previous tab input fields
                txtSweepFrom.TextChanged -= HandleTextFieldDecimalChange;
                txtSweepTo.TextChanged -= HandleTextFieldDecimalChange;
                txtSweepStep.TextChanged -= HandleTextFieldDecimalChange;
                txtVoltageLevel.TextChanged -= HandleTextFieldDecimalChange;
            }

            iCurrentTab = nNewTab;
            
        }

        public void UpdateCurrentTab(int nNewTab, TextBox txtSweepFromNew, TextBox txtSweepToNew, TextBox txtSweepStepNew, TextBox txtVoltageLevelNew)
        {
            this.UpdateCurrentTab(nNewTab);

            txtSweepFrom = txtSweepFromNew;
            txtSweepTo = txtSweepToNew;
            txtSweepStep = txtSweepStepNew;
            txtVoltageLevel = txtVoltageLevelNew;

            LoadTabSettings(nNewTab);

            // add event handlers to previous tab input fields
            txtSweepFrom.TextChanged += HandleTextFieldDecimalChange;
            txtSweepTo.TextChanged += HandleTextFieldDecimalChange;
            txtSweepStep.TextChanged += HandleTextFieldDecimalChange;
            txtVoltageLevel.TextChanged += HandleTextFieldDecimalChange;
        }

        public void StartMeasurement()
        {
            if (!ValidateUserInputValues()) return;

            String sCommandLine = String.Format("/k python {0}.py -from {1} -to {2} -step {3} -V {7} -S \"{4}\" -D {5} -ID {6}",
                strScripts[iCurrentTab], txtSweepFrom.Text, txtSweepTo.Text, txtSweepStep.Text, strSampleName, iDeviceModel, strDeviceID, txtVoltageLevel.Text);

            Process.Start("cmd.exe", sCommandLine);
        }
    }
}
