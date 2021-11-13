using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Shell
{
    public partial class frmMain : Form
    {
        // Settings names in Windows registry
        private const String strCurrentTab = "CurrentTab";
        private const String strSampleName = "SampleName";
        private const String strDeviceID = "RLCMeterID";
        private const String strDeviceModel = "RLCMeterDeviceModel";

        private RunParams meas_params;

        private const int iTabRf = 0;
        private const int iTabLf = 1;
        private const int iTabCf = 2;
        private const int iTabZf = 3;
        private const int iTabParams = 4;

        private const String strError = "Error";

        public frmMain()
        {
            meas_params = new RunParams();
            InitializeComponent();
        }

        private void SaveSettings()
        {
            Settings sett = new Settings();
            sett.SaveSetting(strSampleName, txtSampleName.Text);
            sett.SaveSetting(strDeviceID, txtDeviceID.Text);
            sett.SaveSetting(strDeviceModel, cboRLCMeterModel.SelectedIndex);

            sett.SaveSetting(strCurrentTab, tabControl1.SelectedIndex);
        }

        private void LoadSettings()
        {
            // Initialize generic settings
            String strRead;
            int iRead;

            Settings sett = new Settings();

            strRead = "Test sample";
            sett.TryLoadSetting(strSampleName, ref strRead);
            txtSampleName.Text = strRead;

            strRead = "TCPIPO::0.0.0.0::inst0::INSTR";
            sett.TryLoadSetting(strDeviceID, ref strRead);
            txtDeviceID.Text = strRead;

            iRead = 0;
            sett.TryLoadSetting(strDeviceModel, ref iRead);
            cboRLCMeterModel.SelectedIndex = iRead;

            iRead = 0;
            sett.TryLoadSetting(strCurrentTab, ref iRead);
            tabControl1.SelectedIndex = iRead;

            //initialize current tab settings
            TabChange(tabControl1.SelectedIndex);
        }

        private void CmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveSettings();
        }

        private void CmdOK_Click(object sender, EventArgs e)
        {
            if(tabControl1.SelectedIndex == iTabParams)
            {
                MessageBox.Show("Please select a tab with a needed type of measurement", strError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            meas_params.StartMeasurement();
        }

        private void TabChange(int iNewTab)
        {
            switch(iNewTab)
            {
                case iTabRf:
                    meas_params.UpdateCurrentTab(iNewTab, txtRfSweepFrom, txtRfSweepTo, txtRfSweepStep, txtRfVoltageLevel);
                    break;
                case iTabLf:
                    meas_params.UpdateCurrentTab(iNewTab, txtLfSweepFrom, txtLfSweepTo, txtLfSweepStep, txtLfVoltageLevel);
                    break;
                case iTabCf:
                    meas_params.UpdateCurrentTab(iNewTab, txtCfSweepFrom, txtCfSweepTo, txtCfSweepStep, txtCfVoltageLevel);
                    break;
                case iTabZf:
                    meas_params.UpdateCurrentTab(iNewTab, txtZfSweepFrom, txtZfSweepTo, txtZfSweepStep, txtZfVoltageLevel);
                    break;
            }
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabChange(tabControl1.SelectedIndex);
        }

        private void CboRLCMeterModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            meas_params.SetDeviceModel(cboRLCMeterModel.SelectedIndex);
        }

        private void TxtDeviceID_TextChanged(object sender, EventArgs e)
        {
            meas_params.SetDeviceID(txtDeviceID.Text);
        }

        private void TxtSampleName_TextChanged(object sender, EventArgs e)
        {
            meas_params.SetSampleName(txtSampleName.Text);
        }
    }
}
