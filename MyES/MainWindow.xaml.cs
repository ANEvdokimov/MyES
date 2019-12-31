using Microsoft.Win32;
using System;
using System.Windows;

namespace MyES
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MkbFile mkbFile;
        private MkbFile sourceMkbFile;
        private int numberQuestion;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "mkb files(*.mkb)|*.mkb|All files(*.*)|*.*"
            };

            if (!(bool)openFileDialog.ShowDialog())
            {
                return;
            }

            try
            {
                sourceMkbFile = MkbReader.ReadFile(openFileDialog.FileName);
            }
            catch (Exception exception)
            {
                Console.Write(exception.StackTrace);
                MessageBox.Show(exception.Message);
                return;
            }

            Title = "MyES - " + openFileDialog.FileName;
            Clear();
            DG_Questions.ItemsSource = sourceMkbFile.Questions;
            DG_Result.ItemsSource = sourceMkbFile.Hypotheses;
            TB_Info.Text = sourceMkbFile.InfoToString();

            Menu_MainMenu_Start.IsEnabled = true;
            Menu_MainMenu_Cancel.IsEnabled = false;
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            Menu_MainMenu_Start.IsEnabled = false;
            Menu_MainMenu_Cancel.IsEnabled = true;
            BTN_Enter.IsEnabled = true;
            mkbFile = (MkbFile)sourceMkbFile.Clone();
            TB_CurrentQuestion.Text = mkbFile.Questions[0].Text;
            numberQuestion = 0;
        }

        private void Finish()
        {
            Menu_MainMenu_Start.IsEnabled = true;
            Menu_MainMenu_Cancel.IsEnabled = false;
            TB_CurrentQuestion.Text = "Опрос окончен";
            BTN_Enter.IsEnabled = false;
        }

        private void Cansel(object sender, RoutedEventArgs e)
        {
            Menu_MainMenu_Start.IsEnabled = true;
            Menu_MainMenu_Cancel.IsEnabled = false;

            UpdateWindow(sourceMkbFile);
            TB_CurrentQuestion.Text = "";
            BTN_Enter.IsEnabled = false;
        }

        private void Clear()
        {
            Menu_MainMenu_Start.IsEnabled = true;
            Menu_MainMenu_Cancel.IsEnabled = false;

            DG_Questions.ItemsSource = null;
            DG_Result.ItemsSource = null;
            TB_CurrentQuestion.Text = "";
            BTN_Enter.IsEnabled = false;
        }

        private void UpdateWindow(MkbFile mkbFile)
        {
            DG_Questions.ItemsSource = null;
            DG_Questions.ItemsSource = mkbFile.Questions;
            DG_Result.ItemsSource = null;
            DG_Result.ItemsSource = mkbFile.Hypotheses;
        }

        private void OutputNextQuestion()
        {
            numberQuestion++;
            if (numberQuestion < mkbFile.Questions.Count)
            {
                TB_CurrentQuestion.Text = mkbFile.Questions[numberQuestion].Text;
            }
            else
            {
                Finish();
            }
        }

        private void BTN_Enter_Click(object sender, RoutedEventArgs e)
        {
            int answer;
            try
            {
                answer = int.Parse(TBX_Answer.Text);
            }
            catch (ArgumentNullException exp)
            {
                Console.Write(exp.StackTrace);
                MessageBox.Show(exp.Message);
                return;
            }
            catch (FormatException exp)
            {
                Console.Write(exp.StackTrace);
                MessageBox.Show(exp.Message);
                return;
            }
            catch (OverflowException exp)
            {
                Console.Write(exp.StackTrace);
                MessageBox.Show(exp.Message);
                return;
            }

            if (answer > 5)
            {
                answer = 5;
            }
            if (answer < -5)
            {
                answer = -5;
            }

            if (answer < 0)
            {
                foreach (var hypothesis in mkbFile.Hypotheses)
                {
                    if (hypothesis.PropertiesProbabilities.ContainsKey(numberQuestion + 1))
                    {
                        double pPlus = hypothesis.PropertiesProbabilities[numberQuestion + 1].Yes;
                        double pMinus = hypothesis.PropertiesProbabilities[numberQuestion + 1].No;
                        double pH = hypothesis.Probability;

                        double pHnotE = (1 - pPlus) * pH / ((1 - pPlus) * pH + (1 - pMinus) * (1 - pH));
                        pH = pHnotE + ((answer - (-5)) * (pH - pHnotE)) / (0 - (-5));
                        hypothesis.Probability = pH;
                    }
                }
            }
            else
            {
                foreach (var hypothesis in mkbFile.Hypotheses)
                {
                    if (hypothesis.PropertiesProbabilities.ContainsKey(numberQuestion + 1))
                    {
                        double pPlus = hypothesis.PropertiesProbabilities[numberQuestion + 1].Yes;
                        double pMinus = hypothesis.PropertiesProbabilities[numberQuestion + 1].No;
                        double pH = hypothesis.Probability;

                        double pHE = (pPlus * pH) / (pPlus * pH + pMinus * (1 - pH));
                        pH += ((answer - 0) * (pHE - pH)) / (5 - 0);
                        hypothesis.Probability = pH;
                    }
                }
            }

            mkbFile.Questions[numberQuestion].Answer = answer;
            UpdateWindow(mkbFile);
            OutputNextQuestion();
        }
    }
}
