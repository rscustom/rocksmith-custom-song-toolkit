using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace RocksmithToolkitLib.Extensions
{
    /// <summary>
    /// Customizable Dialog Box to replace standard Message Box with positioning and custom buttons
    /// </summary>
    public partial class BetterDialog2 : Form
    {
        // System Icons example: Bitmap.FromHicon(SystemIcons.Exclamation.Handle)
        // Resource PNG Icons/Images example: Properties.Resources.LedGuitar
        //
        // Sample Usage: 
        // result = BetterDialog2.ShowDialog(null, "Dialog Title", "Button1Text", "Button2Text", "Button3Text", Properties.Resources.IconImage or Bitmap.FromHicon(SystemIcons.Information.Handle), "IconMessage", 150, 150);
        // if (BetterDialog2.ShowDialog(null, "Dialog Title", "Button1Text", "Button2Text", "Button3Text", Properties.Resources.IconImage or Bitmap.FromHicon(SystemIcons.Information.Handle), "IconMessage", 150, 150) == DialogResult.No)
        //
        /// <summary>
        /// Custom Dialog Box to replace standard Message Box with positioning
        /// </summary>
        /// <param name="dialogTitle">Title displayed in dialog frame at top</param>
        /// <param name="dialogIcon">Image displayed left side</param>
        /// <param name="iconMessage">Promenent bold message displayed to right of icon</param>
        /// <param name="dialogMessage">Main dialog message, if 'null' then not displayed</param>
        /// <param name="textDialogButton1">Message box 1 button text, if 'null' then not displayed</param>
        /// <param name="textDialogButton2">Message box 2 button text, if 'null' then not displayed</param>
        /// <param name="textDialogButton3">Message box 3 button text, if 'null' then not displayed</param>
        /// <param name="topFromCenter">Positive pixel distance up from screen center, default location is centered on screen</param>
        /// <param name="leftFromCenter">Positive pixel distance left from screen center, default location is centered on screen</param>
        public static DialogResult ShowDialog(string dialogMessage, string dialogTitle,
            string textDialogButton1, string textDialogButton2, string textDialogButton3, Image dialogIcon, string iconMessage,
            int topFromCenter = 0, int leftFromCenter = 0)
        {
            using (BetterDialog2 dialog = new BetterDialog2(dialogMessage, dialogTitle,
            textDialogButton1, textDialogButton2, textDialogButton3, dialogIcon, iconMessage, topFromCenter, leftFromCenter))
            {
                DialogResult result = dialog.ShowDialog();
                return result;
            }
        }

        /// <summary>
        /// The private constructor. This is only called by the static method ShowDialog.
        /// </summary>
        private BetterDialog2(string dialogMessage, string dialogTitle, string textDialogButton1, string textDialogButton2,
            string textDialogButton3, Image dialogIcon, string iconMessage, int topFromCenter, int leftFromCenter)
        {
            InitializeComponent();

            // set dialog location
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2 - leftFromCenter, (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2 - topFromCenter);

            // standard eye candy
            this.Font = SystemFonts.MessageBoxFont;
            this.ForeColor = SystemColors.WindowText;

            // visual adjustments
            int iconMessageTweak = 2;
            int dialogMessageTweak = 10;
            int heightTweak = 50; // buttons only
            int widthTweak = 30;
            int minIconHeight = 48;
            int minIconWidth = 48;
            int buttonWidth = 100;
            int maxIconHeight = 240; // used if only icon is shown
            int maxIconWidth = 240;

            // load text, and icon (even if null)
            this.Text = dialogTitle;
            pbIcon.Image = dialogIcon;
            lblIconMessage.Text = iconMessage;
            lblDialogMessage.Text = dialogMessage;

            byte checkByte = 0x00;
            if (dialogIcon != null) checkByte += 0x01;
            else pbIcon.Dispose();
            if (iconMessage != null) checkByte += 0x02;
            else lblIconMessage.Dispose();
            if (dialogMessage != null) checkByte += 0x04;
            else lblDialogMessage.Dispose();

            Console.WriteLine("checkByte: " + checkByte.ToString("X"));

            // adjust dialog dimensions based on message strings
            using (Graphics graphics = this.CreateGraphics())
            {
                var iconMessageSize = new SizeF();
                var dialogMessageSize = new SizeF();

                lblIconMessage.Font = new Font(SystemFonts.MessageBoxFont.FontFamily.Name, 10.0f, FontStyle.Bold, GraphicsUnit.Point);
                lblDialogMessage.Font = new Font(SystemFonts.MessageBoxFont.FontFamily.Name, 9.0f, FontStyle.Regular, GraphicsUnit.Point);

                // height automatically takes into account the number of line returns
                iconMessageSize = graphics.MeasureString(iconMessage, lblIconMessage.Font); //, this.lblIconMessage.Width);
                lblIconMessage.Width = (int)iconMessageSize.Width + iconMessageTweak;
                lblIconMessage.Height = (int)iconMessageSize.Height + iconMessageTweak;
                //
                dialogMessageSize = graphics.MeasureString(dialogMessage, lblDialogMessage.Font); // , this.lblDialogMessage.Width);
                lblDialogMessage.Width = (int)dialogMessageSize.Width + widthTweak;
                lblDialogMessage.Height = (int)dialogMessageSize.Height + heightTweak + dialogMessageTweak;

                // customized table layout pannel (must clear)
                tlpDialog.ColumnStyles.Clear();
                tlpDialog.RowStyles.Clear();

                // apply colors for debugging
                //tlpDialog.BackColor = Color.Yellow;
                //lblIconMessage.BackColor = Color.Red;
                //lblDialogMessage.BackColor = Color.Lime;

                switch (checkByte) // eight possible conditions, 2^3=8
                {
                    case 0x00: // no icon and no messages
                        Controls.Remove(tlpDialog);
                        Console.WriteLine("No icon or messages");
                        break;

                    case 0x01: // icon
                        tlpDialog.ColumnCount = 1;
                        tlpDialog.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1.0F));
                        tlpDialog.SetColumn(pbIcon, 0);

                        tlpDialog.RowCount = 1;
                        tlpDialog.RowStyles.Add(new RowStyle(SizeType.Percent, 1.0F));
                        tlpDialog.SetRow(pbIcon, 0);
                        // pbIcon.Dock = DockStyle.Fill;

                        pbIcon.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
                        Console.WriteLine("Icon image");
                        break;

                    case 0x02: // icon message 
                        tlpDialog.ColumnCount = 1;
                        tlpDialog.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1.0F));
                        tlpDialog.SetColumn(lblIconMessage, 0);

                        tlpDialog.RowCount = 1;
                        tlpDialog.RowStyles.Add(new RowStyle(SizeType.Absolute, (int)iconMessageSize.Height + iconMessageTweak));
                        tlpDialog.SetRow(lblIconMessage, 0);

                        lblIconMessage.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
                        heightTweak = heightTweak + iconMessageTweak;
                        Console.WriteLine("Icon message");
                        break;

                    case 0x03: // icon and icon message
                        tlpDialog.RowCount = 1;
                        Console.WriteLine("Icon image and icon message");

                        if (iconMessageSize.Height < minIconHeight)
                            iconMessageSize.Height = minIconHeight;
                        else
                            heightTweak = heightTweak + iconMessageTweak;

                        break;

                    case 0x04: // dialog message            
                        tlpDialog.ColumnCount = 1;
                        tlpDialog.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1.0F));
                        tlpDialog.SetColumn(lblDialogMessage, 0);

                        tlpDialog.RowCount = 1;
                        tlpDialog.RowStyles.Add(new RowStyle(SizeType.Absolute, (int)dialogMessageSize.Height + dialogMessageTweak));
                        tlpDialog.SetRow(lblDialogMessage, 0);

                        lblDialogMessage.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
                        // lblDialogMessage.Padding = new Padding(5, 5, 0, 0);
                        heightTweak = heightTweak + dialogMessageTweak;
                        Console.WriteLine("Dialog message");
                        break;

                    case 0x05: // icon and dialog message
                        tlpDialog.RowCount = 1;
                        tlpDialog.SetRow(lblDialogMessage, 0);
                        Console.WriteLine("Icon image with dialog message");

                        if (dialogMessageSize.Height < minIconHeight)
                            dialogMessageSize.Height = minIconHeight;
                        else
                            heightTweak = heightTweak + dialogMessageTweak;

                        break;

                    case 0x06: // icon message and dialog message
                        tlpDialog.ColumnCount = 1;
                        tlpDialog.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1.0F));
                        tlpDialog.SetColumn(lblIconMessage, 0);
                        tlpDialog.SetColumn(lblDialogMessage, 0);

                        tlpDialog.RowCount = 2;
                        tlpDialog.RowStyles.Add(new RowStyle(SizeType.Absolute, (int)iconMessageSize.Height + iconMessageTweak));
                        tlpDialog.RowStyles.Add(new RowStyle(SizeType.Absolute, (int)dialogMessageSize.Height + dialogMessageTweak));
                        tlpDialog.SetRow(lblIconMessage, 0);
                        tlpDialog.SetRow(lblDialogMessage, 1);

                        lblIconMessage.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
                        lblDialogMessage.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
                        heightTweak = heightTweak + iconMessageTweak + dialogMessageTweak;
                        Console.WriteLine("Icon message and dialog message");
                        break;

                    case 0x07: // icon, icon message and dialog message
                        tlpDialog.ColumnCount = 2;
                        tlpDialog.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40));
                        tlpDialog.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1.0F));
                        tlpDialog.SetColumn(pbIcon, 0);
                        tlpDialog.SetColumn(lblIconMessage, 1);
                        tlpDialog.SetColumn(lblDialogMessage, 1);

                        if ((int)iconMessageSize.Height < minIconHeight)
                            iconMessageSize.Height = minIconHeight;

                        tlpDialog.RowCount = 2;
                        tlpDialog.RowStyles.Add(new RowStyle(SizeType.Absolute, (int)iconMessageSize.Height + iconMessageTweak));
                        tlpDialog.RowStyles.Add(new RowStyle(SizeType.Absolute, (int)dialogMessageSize.Height + dialogMessageTweak));
                        tlpDialog.SetRow(pbIcon, 0);
                        tlpDialog.SetRow(lblIconMessage, 0);
                        tlpDialog.SetRow(lblDialogMessage, 1);

                        lblIconMessage.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
                        lblDialogMessage.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
                        heightTweak = heightTweak + iconMessageTweak + dialogMessageTweak;
                        Console.WriteLine("Icon image, icon message and dialog message");
                        break;

                    default:
                        throw new Exception("Invalid parameter check byte sum");
                }

                // set height according to the message strings, button and tweaks

                this.Height = (int)iconMessageSize.Height + (int)dialogMessageSize.Height +
                    btn3.Height + heightTweak;

                // set width based on the longest text's width
                int bigger = (dialogMessageSize.Width >= iconMessageSize.Width) ? (int)dialogMessageSize.Width : (int)iconMessageSize.Width;

                // this usually produces better results for width
                SizeF sizeIconMsg = TextRenderer.MeasureText("C" + iconMessage, lblIconMessage.Font);
                SizeF sizeDialogMsg = TextRenderer.MeasureText("C" + dialogMessage, lblDialogMessage.Font);
                var sizeC1 = sizeIconMsg.Width <= sizeDialogMsg.Width ? sizeDialogMsg : sizeIconMsg;

                if ((int)sizeC1.Width > (int)bigger)
                    bigger = (int)sizeC1.Width;


                if (dialogIcon == null)
                    this.Width = bigger + widthTweak;
                else
                    this.Width = bigger + minIconWidth + widthTweak;
            }

            int buttonCount = 1;
            if (!String.IsNullOrEmpty(textDialogButton2))
                buttonCount++;
            if (!String.IsNullOrEmpty(textDialogButton1))
                buttonCount++;


            // setup buttons
            switch (buttonCount)
            {
                case 3: // 3 button
                    btn1.Text = textDialogButton1;
                    btn2.Text = textDialogButton2;
                    btn3.Text = textDialogButton3;
                    btn1.Visible = true;
                    btn2.Visible = true;
                    btn3.Visible = true;
                    btn1.DialogResult = DialogResult.Yes;
                    btn2.DialogResult = DialogResult.No;
                    btn3.DialogResult = DialogResult.Cancel;
                    this.AcceptButton = btn1;
                    buttonWidth = buttonWidth * 3;
                    break;
                case 2: // 2 button
                    btn2.Text = textDialogButton2;
                    btn3.Text = textDialogButton3;
                    btn1.Visible = false;
                    btn2.Visible = true;
                    btn2.Visible = true;
                    btn2.DialogResult = DialogResult.Yes;
                    btn3.DialogResult = DialogResult.No;
                    this.AcceptButton = btn2;
                    buttonWidth = buttonWidth * 2;
                    break;
                default:  // 1 button                  
                    btn3.Text = textDialogButton3;
                    btn1.Visible = false;
                    btn2.Visible = false;
                    btn3.Visible = true;
                    btn3.DialogResult = DialogResult.OK;
                    this.AcceptButton = btn3;
                    buttonWidth = buttonWidth * 2; // double wide
                    break;
            }

            // expand depending on number of buttons shown            
            if (this.Width < buttonWidth)
                this.Width = buttonWidth;

            // final adjustment if only icon is shown
            if (checkByte == 0x01)
            {
                if (this.Width < maxIconWidth)
                    this.Width = maxIconWidth;
                if (this.Height < maxIconHeight)
                    this.Height = maxIconHeight;
            }
        }


    }
}
