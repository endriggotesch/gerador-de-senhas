using System;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace GeradorDeSenhasPortable
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GeneratorForm());
        }
    }

    internal sealed class GeneratorForm : Form
    {
        private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
        private const string Numbers = "0123456789";
        private const string Symbols = "!@#$%&*()-_=+[]{};:,.?/";

        private readonly TextBox passwordBox = new TextBox();
        private readonly Button copyButton = new Button();
        private readonly Button generateButton = new Button();
        private readonly TrackBar lengthSlider = new TrackBar();
        private readonly Label lengthValue = new Label();
        private readonly Label strengthValue = new Label();
        private readonly Panel strengthTrack = new Panel();
        private readonly Panel strengthBar = new Panel();
        private readonly CheckBox uppercaseBox = new CheckBox();
        private readonly CheckBox lowercaseBox = new CheckBox();
        private readonly CheckBox numbersBox = new CheckBox();
        private readonly CheckBox symbolsBox = new CheckBox();
        private readonly Label message = new Label();

        private readonly Color background = Color.FromArgb(7, 11, 20);
        private readonly Color panel = Color.FromArgb(16, 24, 39);
        private readonly Color panelSoft = Color.FromArgb(21, 31, 49);
        private readonly Color text = Color.FromArgb(244, 248, 255);
        private readonly Color muted = Color.FromArgb(154, 168, 189);
        private readonly Color line = Color.FromArgb(38, 53, 77);
        private readonly Color accent = Color.FromArgb(59, 130, 246);
        private readonly Color accentDark = Color.FromArgb(29, 78, 216);
        private readonly Color warn = Color.FromArgb(245, 158, 11);
        private readonly Color danger = Color.FromArgb(239, 68, 68);

        public GeneratorForm()
        {
            Text = "Gerador de Senhas";
            ClientSize = new Size(560, 560);
            MinimumSize = new Size(460, 520);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = background;
            ForeColor = text;
            Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            BuildLayout();
            WireEvents();
            GeneratePassword();
        }

        private void BuildLayout()
        {
            var card = new TableLayoutPanel
            {
                BackColor = panel,
                ColumnCount = 1,
                Dock = DockStyle.Fill,
                Padding = new Padding(28),
                RowCount = 9
            };

            card.RowStyles.Add(new RowStyle(SizeType.Absolute, 86));
            card.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
            card.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            card.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            card.RowStyles.Add(new RowStyle(SizeType.Absolute, 130));
            card.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            card.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            card.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            Controls.Add(card);

            var eyebrow = new Label
            {
                AutoSize = true,
                ForeColor = accent,
                Text = "SEGURANCA SIMPLES",
                Font = new Font(Font, FontStyle.Bold),
                Margin = new Padding(0, 0, 0, 5)
            };

            var title = new Label
            {
                AutoSize = true,
                ForeColor = text,
                Text = "Gerador de Senhas",
                Font = new Font("Segoe UI", 26F, FontStyle.Bold, GraphicsUnit.Point),
                Margin = new Padding(0)
            };

            var header = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Margin = new Padding(0)
            };
            header.Controls.Add(eyebrow);
            header.Controls.Add(title);
            card.Controls.Add(header, 0, 0);

            var passwordRow = new TableLayoutPanel
            {
                ColumnCount = 2,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 14)
            };
            passwordRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            passwordRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 104));

            passwordBox.BorderStyle = BorderStyle.FixedSingle;
            passwordBox.BackColor = panelSoft;
            passwordBox.ForeColor = text;
            passwordBox.Font = new Font("Consolas", 12F, FontStyle.Regular, GraphicsUnit.Point);
            passwordBox.ReadOnly = true;
            passwordBox.Dock = DockStyle.Fill;
            passwordBox.Margin = new Padding(0, 3, 10, 3);

            StyleButton(copyButton, "Copiar");
            passwordRow.Controls.Add(passwordBox, 0, 0);
            passwordRow.Controls.Add(copyButton, 1, 0);
            card.Controls.Add(passwordRow, 0, 1);

            var strengthPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                Margin = new Padding(0, 0, 0, 12)
            };
            strengthPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            strengthPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 14));

            var strengthHeader = CreateHeaderRow("Forca", strengthValue);
            strengthTrack.BackColor = Color.FromArgb(30, 41, 59);
            strengthTrack.Dock = DockStyle.Fill;
            strengthTrack.Margin = new Padding(0, 4, 0, 0);
            strengthTrack.Controls.Add(strengthBar);
            strengthBar.Height = 10;
            strengthBar.Left = 0;
            strengthBar.Top = 0;

            strengthPanel.Controls.Add(strengthHeader, 0, 0);
            strengthPanel.Controls.Add(strengthTrack, 0, 1);
            card.Controls.Add(strengthPanel, 0, 2);

            var lengthPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                Margin = new Padding(0, 0, 0, 12)
            };
            lengthPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            lengthPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            lengthSlider.Minimum = 6;
            lengthSlider.Maximum = 40;
            lengthSlider.Value = 16;
            lengthSlider.TickFrequency = 2;
            lengthSlider.BackColor = panel;
            lengthSlider.Dock = DockStyle.Fill;
            lengthPanel.Controls.Add(CreateHeaderRow("Tamanho", lengthValue), 0, 0);
            lengthPanel.Controls.Add(lengthSlider, 0, 1);
            card.Controls.Add(lengthPanel, 0, 3);

            var options = new TableLayoutPanel
            {
                ColumnCount = 2,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 16),
                RowCount = 3
            };
            options.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            options.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            options.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            options.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            options.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));

            var legend = new Label
            {
                Text = "Caracteres",
                ForeColor = muted,
                Dock = DockStyle.Fill,
                Font = new Font(Font, FontStyle.Bold)
            };
            options.Controls.Add(legend, 0, 0);
            options.SetColumnSpan(legend, 2);

            ConfigureOption(uppercaseBox, "Letras maiusculas");
            ConfigureOption(lowercaseBox, "Letras minusculas");
            ConfigureOption(numbersBox, "Numeros");
            ConfigureOption(symbolsBox, "Simbolos");
            options.Controls.Add(uppercaseBox, 0, 1);
            options.Controls.Add(lowercaseBox, 1, 1);
            options.Controls.Add(numbersBox, 0, 2);
            options.Controls.Add(symbolsBox, 1, 2);
            card.Controls.Add(options, 0, 4);

            StyleButton(generateButton, "Gerar senha");
            card.Controls.Add(generateButton, 0, 5);

            message.Dock = DockStyle.Fill;
            message.ForeColor = muted;
            message.TextAlign = ContentAlignment.MiddleCenter;
            card.Controls.Add(message, 0, 6);
        }

        private void WireEvents()
        {
            copyButton.Click += delegate { CopyPassword(); };
            generateButton.Click += delegate { GeneratePassword(); };
            lengthSlider.ValueChanged += delegate
            {
                UpdateStrength();
                GeneratePassword();
            };

            uppercaseBox.CheckedChanged += delegate { GeneratePassword(); };
            lowercaseBox.CheckedChanged += delegate { GeneratePassword(); };
            numbersBox.CheckedChanged += delegate { GeneratePassword(); };
            symbolsBox.CheckedChanged += delegate { GeneratePassword(); };
            strengthTrack.Resize += delegate { UpdateStrength(); };
        }

        private TableLayoutPanel CreateHeaderRow(string labelText, Label valueLabel)
        {
            var row = new TableLayoutPanel { ColumnCount = 2, Dock = DockStyle.Fill, Margin = new Padding(0) };
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));

            var label = new Label
            {
                Text = labelText,
                ForeColor = muted,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            valueLabel.ForeColor = text;
            valueLabel.Dock = DockStyle.Fill;
            valueLabel.TextAlign = ContentAlignment.MiddleRight;
            row.Controls.Add(label, 0, 0);
            row.Controls.Add(valueLabel, 1, 0);
            return row;
        }

        private void ConfigureOption(CheckBox checkbox, string label)
        {
            checkbox.Appearance = Appearance.Normal;
            checkbox.Checked = true;
            checkbox.Text = label;
            checkbox.ForeColor = text;
            checkbox.BackColor = panelSoft;
            checkbox.Dock = DockStyle.Fill;
            checkbox.Margin = new Padding(0, 4, 10, 4);
            checkbox.Padding = new Padding(10, 0, 0, 0);
        }

        private void StyleButton(Button button, string label)
        {
            button.Text = label;
            button.BackColor = accent;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Dock = DockStyle.Fill;
            button.Margin = new Padding(0, 3, 0, 3);
            button.Font = new Font(Font, FontStyle.Bold);
            button.Cursor = Cursors.Hand;
            button.MouseEnter += delegate { button.BackColor = accentDark; };
            button.MouseLeave += delegate { button.BackColor = accent; };
        }

        private void GeneratePassword()
        {
            var selectedSets = GetSelectedSets();
            lengthValue.Text = lengthSlider.Value.ToString();

            if (selectedSets.Length == 0)
            {
                passwordBox.Text = "";
                message.Text = "Selecione pelo menos um tipo de caractere.";
                UpdateStrength();
                return;
            }

            var password = string.Concat(selectedSets.Select(set => set[RandomIndex(set.Length)]));
            var allCharacters = string.Concat(selectedSets);

            while (password.Length < lengthSlider.Value)
            {
                password += allCharacters[RandomIndex(allCharacters.Length)];
            }

            passwordBox.Text = Shuffle(password.Substring(0, lengthSlider.Value));
            message.Text = "Senha gerada.";
            UpdateStrength();
        }

        private void CopyPassword()
        {
            if (string.IsNullOrWhiteSpace(passwordBox.Text))
            {
                message.Text = "Gere uma senha antes de copiar.";
                return;
            }

            Clipboard.SetText(passwordBox.Text);
            message.Text = "Senha copiada.";
        }

        private string[] GetSelectedSets()
        {
            return new[]
            {
                uppercaseBox.Checked ? Uppercase : "",
                lowercaseBox.Checked ? Lowercase : "",
                numbersBox.Checked ? Numbers : "",
                symbolsBox.Checked ? Symbols : ""
            }.Where(set => set.Length > 0).ToArray();
        }

        private void UpdateStrength()
        {
            var selectedCount = GetSelectedSets().Length;
            var score = lengthSlider.Value + selectedCount * 5;
            var width = 0.28;
            var color = danger;
            var label = "Fraca";

            if (score >= 32)
            {
                width = 1;
                color = accent;
                label = "Forte";
            }
            else if (score >= 18)
            {
                width = 0.65;
                color = warn;
                label = "Boa";
            }

            strengthValue.Text = label;
            strengthBar.BackColor = color;
            strengthBar.Width = Math.Max(12, (int)(strengthTrack.ClientSize.Width * width));
        }

        private static int RandomIndex(int max)
        {
            if (max <= 0)
            {
                return 0;
            }

            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                return (int)(BitConverter.ToUInt32(bytes, 0) % max);
            }
        }

        private static string Shuffle(string text)
        {
            var characters = text.ToCharArray();

            for (var index = characters.Length - 1; index > 0; index--)
            {
                var swapIndex = RandomIndex(index + 1);
                var current = characters[index];
                characters[index] = characters[swapIndex];
                characters[swapIndex] = current;
            }

            return new string(characters);
        }
    }
}
