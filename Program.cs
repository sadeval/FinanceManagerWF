using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FinancialApp;
using FinancialAppWindowsForms;
using Newtonsoft.Json;

namespace FinancialApp
{
    /// <summary>
    /// Абстрактный класс, представляющий транзакцию.
    /// </summary>
    public abstract class Transaction
    {
        /// <summary>
        /// Уникальный идентификатор транзакции.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Сумма транзакции.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Дата транзакции.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Описание транзакции.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Валюта транзакции.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Тип транзакции (Приход или Расход).
        /// </summary>
        public string TransactionType { get; set; }

        /// <summary>
        /// Конструктор для создания транзакции.
        /// </summary>
        /// <param name="id">Идентификатор транзакции.</param>
        /// <param name="amount">Сумма транзакции.</param>
        /// <param name="date">Дата транзакции.</param>
        /// <param name="description">Описание транзакции.</param>
        /// <param name="currency">Валюта транзакции.</param>
        /// <param name="transactionType">Тип транзакции.</param>
        protected Transaction(int id, decimal amount, DateTime date, string description, string currency, string transactionType)
        {
            Id = id;
            Amount = amount;
            Date = date;
            Description = description;
            Currency = currency;
            TransactionType = transactionType;
        }
    }

    /// <summary>
    /// Класс для представления доходов.
    /// </summary>
    public class Income : Transaction

    {
        /// <summary>
        /// Конструктор для создания доходной транзакции.
        /// </summary>
        /// <param name="id">Идентификатор транзакции.</param>
        /// <param name="amount">Сумма дохода.</param>
        /// <param name="date">Дата дохода.</param>
        /// <param name="description">Описание дохода.</param>
        /// <param name="currency">Валюта дохода.</param>
        /// <param name="transactionType">Тип дохода.</param>
        public Income(int id, decimal amount, DateTime date, string description, string currency, string transactionType)
            : base(id, amount, date, description, currency, transactionType)
        {
        }
    }

    /// <summary>
    /// Класс для представления расходов.
    /// </summary>
    public class Expense : Transaction
    {
        /// <summary>
        /// Конструктор для создания расходной транзакции.
        /// </summary>
        /// <param name="id">Идентификатор транзакции.</param>
        /// <param name="amount">Сумма расхода.</param>
        /// <param name="date">Дата расхода.</param>
        /// <param name="description">Описание расхода.</param>
        /// <param name="currency">Валюта расхода.</param>
        /// <param name="transactionType">Тип расхода.</param>
        public Expense(int id, decimal amount, DateTime date, string description, string currency, string transactionType)
            : base(id, amount, date, description, currency, transactionType)
        {
        }
    }

    /// <summary>
    /// Класс, управляющий списком транзакций и выполнением операций с ними.
    /// </summary>
    public class FinancialManager
    {
        private List<Transaction> transactions = new List<Transaction>();

        /// <summary>
        /// Добавляет новую транзакцию в список.
        /// </summary>
        /// <param name="transaction">Транзакция для добавления.</param>
        public void AddTransaction(Transaction transaction)
        {
            transactions.Add(transaction);
            RenumberTransactionIds(); // Перенумеровываем ID после добавления
        }

        /// <summary>
        /// Удаляет транзакцию по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор транзакции для удаления.</param>
        public void DeleteTransaction(int id)
        {
            var transaction = transactions.FirstOrDefault(t => t.Id == id);
            if (transaction != null)
            {
                transactions.Remove(transaction);
                RenumberTransactionIds(); // Перенумеровываем ID после удаления
            }
        }

        /// <summary>
        /// Возвращает список всех транзакций.
        /// </summary>
        /// <returns>Список транзакций.</returns>
        public List<Transaction> GetTransactions()
        {
            return transactions;
        }

        /// <summary>
        /// Загружает транзакции в список.
        /// </summary>
        /// <param name="loadedTransactions">Список загружаемых транзакций.</param>
        public void LoadTransactions(List<Transaction> loadedTransactions)
        {
            transactions = loadedTransactions;
            RenumberTransactionIds(); // Перенумеровываем ID после загрузки
        }

        /// <summary>
        /// Рассчитывает общий баланс на основе всех транзакций.
        /// </summary>
        /// <returns>Общий баланс.</returns>
        public decimal GetBalance()
        {
            return transactions.Sum(t => t is Income ? t.Amount : -t.Amount);
        }

        /// <summary>
        /// Возвращает следующий доступный идентификатор транзакции.
        /// </summary>
        /// <returns>Следующий доступный идентификатор.</returns>
        public int GetNextId()
        {
            return transactions.Any() ? transactions.Max(t => t.Id) + 1 : 1;
        }

        /// <summary>
        /// Возвращает валюту первой транзакции в списке или "UAH", если транзакций нет.
        /// </summary>
        /// <returns>Валюта транзакции.</returns>
        public string GetCurrency()
        {
            return transactions.FirstOrDefault()?.Currency ?? "UAH";
        }

        /// <summary>
        /// Перенумеровывает идентификаторы транзакций в списке.
        /// </summary>
        private void RenumberTransactionIds()
        {
            for (int i = 0; i < transactions.Count; i++)
            {
                transactions[i].Id = i + 1;
            }
        }
    }
}


namespace FinancialAppWindowsForms
{
    /// <summary>
    /// Главная форма приложения, отвечающая за отображение интерфейса пользователя.
    /// </summary>
    public partial class MainForm : Form
    {
        private FinancialManager financialManager = new FinancialManager();
        private string transactionsFilePath = "transactions.json";

        private TextBox txtAmount;
        private TextBox txtDescription;
        private TextBox txtDate;
        private ComboBox cmbTransactionType;
        private RadioButton rbtnIncome;
        private RadioButton rbtnExpense;
        private TextBox txtTransactionId;
        private DataGridView dgvTransactions;
        private Label lblBalance;
        private Button btnAddTransaction;
        private Button btnDeleteTransaction;
        private Button btnSearchTransaction;
        private Button btnShowAllTransactions;
        private Button btnShowBalance;
        private Button btnClearFields;
        private DateTimePicker dateTimePicker;

        /// <summary>
        /// Конструктор формы, инициализирующий компонент и загружающий транзакции из файла.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            this.Text = "Financial Manager";
            this.Size = new System.Drawing.Size(1000, 600);
            this.BackColor = System.Drawing.Color.PowderBlue;
            this.Icon = new System.Drawing.Icon("Calc.ico");
            LoadTransactionsFromFile();
        }

        /// <summary>
        /// Инициализирует компоненты формы и задает их свойства.
        /// </summary>
        private void InitializeComponent()
        {
            // Метка для суммы
            Label lblAmount = new Label
            {
                Location = new System.Drawing.Point(20, 20),
                Text = "Сумма",
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold)
            };

            // Текстовое поле для ввода суммы
            txtAmount = new TextBox
            {
                Location = new System.Drawing.Point(20, 45),
                Width = 200,
                Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Bold)
            };

            // Метка для описания
            Label lblDescription = new Label
            {
                Location = new System.Drawing.Point(20, 80),
                Text = "Описание",
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold)
            };

            // Текстовое поле для ввода описания
            txtDescription = new TextBox
            {
                Location = new System.Drawing.Point(20, 105),
                Width = 200,
                Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Bold)
            };

            // Метка для даты
            Label lblDate = new Label
            {
                Location = new System.Drawing.Point(20, 140),
                Text = "Дата",
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold)
            };

            // Поле выбора даты
            dateTimePicker = new DateTimePicker
            {
                Location = new System.Drawing.Point(20, 165),
                Width = 200,
                Format = DateTimePickerFormat.Short,
                Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Bold)
            };

            // Радио-кнопка для типа транзакции "Доход"
            rbtnIncome = new RadioButton
            {
                Location = new System.Drawing.Point(20, 210),
                Text = "Приход",
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold)
            };
            rbtnIncome.CheckedChanged += RbtnIncome_CheckedChanged;

            // Радио-кнопка для типа транзакции "Расход"
            rbtnExpense = new RadioButton
            {
                Location = new System.Drawing.Point(150, 210),
                Text = "Расход",
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold)
            };
            rbtnExpense.CheckedChanged += RbtnExpense_CheckedChanged;

            // Метка для типа транзакции
            Label lblTransactionType = new Label
            {
                Location = new System.Drawing.Point(20, 260),
                Text = "Тип транзакции",
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold)
            };

            // Комбо-бокс для выбора типа транзакции
            cmbTransactionType = new ComboBox
            {
                Location = new System.Drawing.Point(20, 285),
                Width = 200,
                Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Bold)
            };

            // Метка для ID транзакции
            Label lblTransactionId = new Label
            {
                Location = new System.Drawing.Point(20, 325),
                Text = "ID транзакции",
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold)
            };

            // Текстовое поле для ID транзакции
            txtTransactionId = new TextBox
            {
                Location = new System.Drawing.Point(20, 350),
                Width = 200,
                Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Bold)
            };

            // Таблица для отображения списка транзакций
            dgvTransactions = new DataGridView
            {
                Location = new System.Drawing.Point(270, 20),
                Width = 700,
                Height = 350,
                AutoGenerateColumns = false,
                BackgroundColor = System.Drawing.Color.Lavender,
                AlternatingRowsDefaultCellStyle = { BackColor = System.Drawing.Color.Linen },
                DefaultCellStyle = { BackColor = System.Drawing.Color.White },
                ColumnHeadersDefaultCellStyle = { Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold) },
                RowsDefaultCellStyle = { SelectionBackColor = System.Drawing.Color.PowderBlue, SelectionForeColor = System.Drawing.Color.Black, 
                Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold) }
            };


            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 35 });
            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Amount", HeaderText = "Сумма", Width = 80 });
            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Currency", HeaderText = "Валюта", Width = 65 });
            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Date", HeaderText = "Дата", Width = 85 });
            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Description", HeaderText = "Описание", Width = 205 });
            dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TransactionType", HeaderText = "Тип транзакции", Width = 185 });

            // Метка для отображения баланса
            lblBalance = new Label
            {
                Location = new System.Drawing.Point(750, 400),
                Width = 200,
                Text = "Баланс: 0 UAH",
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold)
            };

            // Кнопка для добавления транзакции
            btnAddTransaction = new Button
            {
                Location = new System.Drawing.Point(20, 480),
                Width = 140,
                Height = 50,
                Text = "Добавить транзакцию",
                BackColor = System.Drawing.Color.Linen,
                ForeColor = System.Drawing.Color.Black,
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold)
            };
            btnAddTransaction.Click += BtnAddTransaction_Click;

            // Кнопка для удаления транзакции
            btnDeleteTransaction = new Button
            {
                Location = new System.Drawing.Point(180, 480),
                Width = 140,
                Height = 50,
                Text = "Удалить транзакцию",
                BackColor = System.Drawing.Color.Linen,
                ForeColor = System.Drawing.Color.Black,
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold)
            };
            btnDeleteTransaction.Click += BtnDeleteTransaction_Click;

            // Кнопка для поиска транзакции
            btnSearchTransaction = new Button
            {
                Location = new System.Drawing.Point(340, 480),
                Width = 140,
                Height = 50,
                Text = "Поиск",
                BackColor = System.Drawing.Color.Linen,
                ForeColor = System.Drawing.Color.Black,
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold)
            };
            btnSearchTransaction.Click += BtnSearchTransaction_Click;

            // Кнопка для отображения всех транзакций
            btnShowAllTransactions = new Button
            {
                Location = new System.Drawing.Point(500, 480),
                Width = 140,
                Height = 50,
                Text = "Показать все транзакции",
                BackColor = System.Drawing.Color.Linen,
                ForeColor = System.Drawing.Color.Black,
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold)
            };
            btnShowAllTransactions.Click += BtnShowAllTransactions_Click;

            // Кнопка для отображения баланса
            btnShowBalance = new Button
            {
                Location = new System.Drawing.Point(660, 480),
                Width = 140,
                Height = 50,
                Text = "Показать баланс",
                BackColor = System.Drawing.Color.Linen,
                ForeColor = System.Drawing.Color.Black,
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold)
            };
            btnShowBalance.Click += BtnShowBalance_Click;

            // Кнопка для очистки полей ввода
            btnClearFields = new Button
            {
                Location = new System.Drawing.Point(820, 480),
                Width = 140,
                Height = 50,
                Text = "Очистить поля",
                BackColor = System.Drawing.Color.Linen,
                ForeColor = System.Drawing.Color.Black,
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold)
            };
            btnClearFields.Click += BtnClearFields_Click;

            // Добавление элементов управления на форму
            this.Controls.Add(lblAmount);
            this.Controls.Add(txtAmount);
            this.Controls.Add(lblDescription);
            this.Controls.Add(txtDescription);
            this.Controls.Add(lblDate);
            this.Controls.Add(txtDate);
            this.Controls.Add(rbtnIncome);
            this.Controls.Add(rbtnExpense);
            this.Controls.Add(lblTransactionType);
            this.Controls.Add(cmbTransactionType);
            this.Controls.Add(lblTransactionId);
            this.Controls.Add(txtTransactionId);
            this.Controls.Add(dgvTransactions);
            this.Controls.Add(lblBalance);
            this.Controls.Add(btnAddTransaction);
            this.Controls.Add(btnDeleteTransaction);
            this.Controls.Add(btnSearchTransaction);
            this.Controls.Add(btnShowAllTransactions);
            this.Controls.Add(btnShowBalance);
            this.Controls.Add(btnClearFields);
            this.Controls.Add(dateTimePicker);
        }

        /// <summary>
        /// Обрабатывает изменение состояния радио-кнопки <c>rbtnIncome</c>.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Данные о событии.</param>
        /// <remarks>
        /// При выборе радио-кнопки <c>rbtnIncome</c> очищается выпадающий список <c>cmbTransactionType</c>
        /// и добавляются типы доходов. Также вызывается метод <c>UpdateBalance</c> для обновления баланса.
        /// </remarks>
        private void RbtnIncome_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnIncome.Checked)
            {
                cmbTransactionType.Items.Clear();
                cmbTransactionType.Items.AddRange(new string[] { "Выигрыш", "Зарплата", "Подарок", "Премия", "Другие доходы" });
                UpdateBalance();
            }
        }

        /// <summary>
        /// Обрабатывает изменение состояния радио-кнопки <c>rbtnExpense</c>.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Данные о событии.</param>
        /// <remarks>
        /// При выборе радио-кнопки <c>rbtnExpense</c> очищается выпадающий список <c>cmbTransactionType</c>
        /// и добавляются типы расходов. Также вызывается метод <c>UpdateBalance</c> для обновления баланса.
        /// </remarks>
        private void RbtnExpense_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnExpense.Checked)
            {
                cmbTransactionType.Items.Clear();
                cmbTransactionType.Items.AddRange(new string[] { "Аренда", "Еда", "Квартплата", "Коммунальные услуги", "Кредит", "Машина", "Пожертвование", "Покупки", "Путешествия", "Связь и интернет", "Другие расходы" });
                UpdateBalance();
            }
        }

        /// <summary>
        /// Проверяет корректность введенных данных.
        /// </summary>
        /// <param name="errorMessage">Сообщение об ошибке, если валидация не пройдена.</param>
        /// <returns>
        /// Возвращает <c>true</c>, если все поля заполнены корректно; иначе <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Метод проверяет следующие поля:
        /// <list type="bullet">
        /// <item><description>Корректность формата суммы в <c>txtAmount</c>.</description></item>
        /// <item><description>Корректность формата даты в <c>txtDate</c>.</description></item>
        /// <item><description>Наличие текста в поле <c>txtDescription</c>.</description></item>
        /// <item><description>Выбор типа транзакции в <c>cmbTransactionType</c>.</description></item>
        /// <item><description>Выбор типа транзакции (Приход или Расход) с помощью радио-кнопок.</description></item>
        /// </list>
        /// Если какое-либо поле заполнено некорректно, метод возвращает <c>false</c>
        /// и заполняет параметр <paramref name="errorMessage"/> соответствующим сообщением.
        /// </remarks>
        private bool ValidateInput(out string errorMessage)
        {
            errorMessage = string.Empty;

            // Проверка суммы
            if (!decimal.TryParse(txtAmount.Text, out _))
            {
                errorMessage += "Неверный формат суммы. Пожалуйста, введите числовое значение.\n";
            }

            // Проверка даты
            if (!DateTime.TryParse(dateTimePicker.Text, out _))
            {
                errorMessage += "Неверный формат даты. Введите в формате dd.mm.yyyy.\n";
            }

            // Проверка описания
            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                errorMessage += "Описание не должно быть пустым.\n";
            }

            // Проверка типа транзакции
            if (cmbTransactionType.SelectedItem == null)
            {
                errorMessage += "Выберите тип транзакции.\n";
            }

            // Проверка выбора типа транзакции (приход или расход)
            if (!rbtnIncome.Checked && !rbtnExpense.Checked)
            {
                errorMessage += "Выберите тип транзакции (Приход или Расход).\n";
            }

            return string.IsNullOrEmpty(errorMessage);
        }

        /// <summary>
        /// Обработчик события для кнопки добавления транзакции.
        /// </summary>
        private void BtnAddTransaction_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInput(out string errorMessage))
                {
                    MessageBox.Show(errorMessage, "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int id = financialManager.GetNextId();
                decimal amount = Convert.ToDecimal(txtAmount.Text);
                DateTime date = DateTime.Parse(dateTimePicker.Text);
                string description = txtDescription.Text;
                string currency = financialManager.GetCurrency();
                string transactionType = cmbTransactionType.SelectedItem.ToString();

                Transaction transaction;
                if (rbtnIncome.Checked)
                {
                    transaction = new Income(id, amount, date, description, currency, transactionType);
                }
                else if (rbtnExpense.Checked)
                {
                    transaction = new Expense(id, amount, date, description, currency, transactionType);
                }
                else
                {
                    MessageBox.Show("Выберите тип транзакции (Приход или Расход).");
                    return;
                }

                financialManager.AddTransaction(transaction);
                SaveTransactionsToFile();
                UpdateTransactionsGrid();
                ClearFields();
                UpdateBalance();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении транзакции: " + ex.Message);
            }
        }


        /// <summary>
        /// Обработчик события для кнопки удаления транзакции.
        /// </summary>
        private void BtnDeleteTransaction_Click(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(txtTransactionId.Text, out int transactionId))
                {
                    financialManager.DeleteTransaction(transactionId);
                    SaveTransactionsToFile();
                    UpdateTransactionsGrid();
                    ClearFields();
                    UpdateBalance();
                }
                else
                {
                    MessageBox.Show("Введите правильный ID транзакции.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении транзакции: {ex.Message}");
            }
        }

        /// <summary>
        /// Обработчик события для кнопки поиска транзакции.
        /// </summary>
        private void BtnSearchTransaction_Click(object sender, EventArgs e)
        {
            try
            {
                var filteredTransactions = financialManager.GetTransactions();

                // Фильтрация по дате, если указана конкретная дата
                if (dateTimePicker.Value.Date != DateTime.Now.Date) // Проверяем, выбрана ли конкретная дата
                {
                    DateTime selectedDate = dateTimePicker.Value.Date;
                    filteredTransactions = filteredTransactions.Where(t => t.Date.Date == selectedDate).ToList();
                    // Оставляем только те транзакции, которые соответствуют выбранной дате
                }

                // Фильтрация по типу транзакции, если выбран тип
                if (cmbTransactionType.SelectedItem != null)
                {
                    string selectedType = cmbTransactionType.SelectedItem.ToString(); // Получаем выбранный тип транзакции
                    filteredTransactions = filteredTransactions.Where(t => t.TransactionType == selectedType).ToList();
                    // Оставляем только те транзакции, которые соответствуют выбранному типу
                }

                // Фильтрация по типу операции (доход или расход) на основе выбора радио-кнопки
                if (rbtnIncome.Checked)
                {
                    filteredTransactions = filteredTransactions.Where(t => t is Income).ToList();
                }
                else if (rbtnExpense.Checked)
                {
                    filteredTransactions = filteredTransactions.Where(t => t is Expense).ToList();
                }

                // Обновляем источник данных таблицы транзакций
                dgvTransactions.DataSource = null;
                dgvTransactions.DataSource = filteredTransactions;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска транзакций: {ex.Message}");
            }
        }

        /// <summary>
        /// Обработчик события для кнопки отображения всех транзакций.
        /// </summary>
        private void BtnShowAllTransactions_Click(object sender, EventArgs e)
        {
            UpdateTransactionsGrid();
        }

        /// <summary>
        /// Обработчик события для кнопки отображения баланса.
        /// </summary>
        private void BtnShowBalance_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Баланс: {financialManager.GetBalance()} {financialManager.GetCurrency()}", "Баланс");
        }

        /// <summary>
        /// Обработчик события для кнопки очистки полей ввода.
        /// </summary>
        private void BtnClearFields_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        /// <summary>
        /// Загружает транзакции из файла.
        /// </summary>
        private void LoadTransactionsFromFile()
        {
            if (File.Exists(transactionsFilePath))
            {
                try
                {
                    string json = File.ReadAllText(transactionsFilePath);
                    var loadedTransactions = JsonConvert.DeserializeObject<List<Transaction>>(json, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                    financialManager.LoadTransactions(loadedTransactions);
                    UpdateTransactionsGrid();
                    UpdateBalance();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки транзакций: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Сохраняет транзакции в файл.
        /// </summary>
        private void SaveTransactionsToFile()
        {
            try
            {
                var transactions = financialManager.GetTransactions();
                string json = JsonConvert.SerializeObject(transactions, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented
                });
                File.WriteAllText(transactionsFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения транзакций: {ex.Message}");
            }
        }

        /// <summary>
        /// Обновляет таблицу транзакций на форме.
        /// </summary>
        private void UpdateTransactionsGrid()
        {
            dgvTransactions.DataSource = null;
            dgvTransactions.DataSource = financialManager.GetTransactions();
        }

        /// <summary>
        /// Обновляет отображение баланса на форме.
        /// </summary>
        private void UpdateBalance()
        {
            decimal balance = 0;
            string currency = financialManager.GetCurrency();

            try
            {
                if (rbtnIncome.Checked)
                {
                    // Если выбрана радио-кнопка "Приход", рассчитываем сумму всех приходов
                    balance = financialManager.GetTransactions().Where(t => t is Income).Sum(t => t.Amount);
                    lblBalance.Text = $"Приход: {balance} {currency}"; // Обновляем текст метки для отображения прихода
                }
                else if (rbtnExpense.Checked)
                {
                    // Если выбрана радио-кнопка "Расход", рассчитываем сумму всех расходов
                    balance = financialManager.GetTransactions().Where(t => t is Expense).Sum(t => t.Amount);
                    lblBalance.Text = $"Расход: -{balance} {currency}"; // Обновляем текст метки для отображения расхода
                }
                else
                {
                    // Если не выбрана ни одна радио-кнопка, показываем общий баланс
                    balance = financialManager.GetBalance();
                    lblBalance.Text = $"Баланс: {balance} {currency}"; // Обновляем текст метки для отображения общего баланса
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при расчете баланса: {ex.Message}");
            }
        }

        /// <summary>
        /// Очищает все поля ввода на форме.
        /// </summary>
        private void ClearFields()
        {
            txtAmount.Text = "";
            txtDescription.Text = "";
            dateTimePicker.Value = DateTime.Now;
            txtTransactionId.Text = "";
            cmbTransactionType.SelectedItem = null;
            rbtnIncome.Checked = false;
            rbtnExpense.Checked = false;
        }
    }
}

namespace FinancialApp
{
    /// <summary>
    /// Основной класс программы.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Точка входа в приложение.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles(); // Включаем визуальные стили
            Application.SetCompatibleTextRenderingDefault(false); // Устанавливаем совместимость рендеринга текста
            Application.Run(new MainForm()); // Запускаем главную форму приложения
        }
    }
}
