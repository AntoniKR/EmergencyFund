// Страница вкладов. Все адреса относительные: страница и API на одном origin, поэтому CORS не нужен.
const API = '/api/deposits';

const typeLabels = {
  deposit: 'Вклад',
  savingsAccount: 'Накопительный счёт'
};

const capitalizationLabels = {
  none: 'без капитализации',
  daily: 'ежедневная капитализация',
  weekly: 'еженедельная капитализация',
  monthly: 'ежемесячная капитализация'
};

const ledger = document.getElementById('ledger');
const listMessage = document.getElementById('list-message');
const total = document.getElementById('total');
const totalNote = document.getElementById('total-note');

const form = document.getElementById('deposit-form');
const fields = {
  name: document.getElementById('field-name'),
  type: document.getElementById('field-type'),
  capitalization: document.getElementById('field-capitalization'),
  sum: document.getElementById('field-sum'),
  rate: document.getElementById('field-rate'),
  start: document.getElementById('field-start'),
  term: document.getElementById('field-term')
};
const termField = document.getElementById('term-field');
const formErrors = document.getElementById('form-errors');
const submitButton = document.getElementById('submit-button');

function showListMessage(text) {
  listMessage.textContent = text;
}

// ---------- Список ----------

async function loadDeposits() {
  try {
    const response = await fetch(API);
    if (!response.ok) {
      showListMessage(`Список не загрузился: сервер ответил ${response.status}.`);
      return;
    }
    renderDeposits(await response.json());
  } catch {
    showListMessage('Нет связи с сервером. Проверьте, что приложение запущено, и обновите страницу.');
  }
}

function renderDeposits(deposits) {
  ledger.replaceChildren();

  const totalSum = deposits.reduce((acc, deposit) => acc + deposit.sum, 0);
  total.textContent = money.format(totalSum);

  const depositCount = deposits.filter(d => d.type === 'deposit').length;
  const accountCount = deposits.length - depositCount;
  totalNote.textContent = deposits.length === 0
    ? ''
    : `${depositCount} ${plural(depositCount, ['вклад', 'вклада', 'вкладов'])} и ${accountCount} ${plural(accountCount, ['счёт', 'счёта', 'счетов'])}`;

  if (deposits.length === 0) {
    ledger.append(el('p', 'empty', 'Здесь появятся ваши вклады и счета. Добавьте первый через форму ниже.'));
    return;
  }

  // Порядок приходит с сервера: по дате открытия, при равенстве по id (так записано в контракте).
  for (const deposit of deposits) {
    ledger.append(createEntry(deposit));
  }
}

function createEntry(deposit) {
  const entry = el('article', 'entry');

  const head = el('div');
  head.append(el('h3', 'entry-name', deposit.name));
  const typeLabel = typeLabels[deposit.type] ?? deposit.type;
  const capitalizationLabel = capitalizationLabels[deposit.capitalization] ?? deposit.capitalization;
  head.append(el('p', 'entry-kind', `${typeLabel}, ${capitalizationLabel}`));

  const deleteButton = el('button', 'button-danger', 'Удалить');
  deleteButton.type = 'button';
  deleteButton.setAttribute('aria-label', `Удалить «${deposit.name}»`);
  deleteButton.addEventListener('click', () => deleteDeposit(deposit));

  const figures = el('dl', 'figures');
  addFigure(figures, 'Сумма', money.format(deposit.sum));
  addFigure(figures, 'Ставка', `${percent.format(deposit.interestRate)} % годовых`);
  addFigure(figures, 'Открыт', formatDate(deposit.startDate));

  if (deposit.term === null) {
    addFigure(figures, 'Срок', 'Без срока');
  } else {
    const months = `${deposit.term} ${plural(deposit.term, ['месяц', 'месяца', 'месяцев'])}`;
    addFigure(figures, 'Срок', `${months}, до ${formatDate(deposit.endDate)}`);
  }

  if (deposit.estimatedSum === null) {
    addFigure(figures, 'К концу срока', 'Не считается для счёта без срока');
  } else {
    addFigure(figures, 'К концу срока', el('span', 'stamp', money.format(deposit.estimatedSum)));
  }

  entry.append(head, deleteButton, figures);
  return entry;
}

async function deleteDeposit(deposit) {
  if (!confirm(`Удалить «${deposit.name}»?`)) {
    return;
  }
  try {
    const response = await fetch(`${API}/${deposit.id}`, { method: 'DELETE' });
    if (response.status === 204) {
      showListMessage(`Удалено: «${deposit.name}».`);
    } else if (response.status === 404) {
      showListMessage(`«${deposit.name}» уже удалён, список обновлён.`);
    } else {
      showListMessage(`Не удалось удалить: сервер ответил ${response.status}.`);
    }
    await loadDeposits();
  } catch {
    showListMessage('Нет связи с сервером. Ничего не удалено.');
  }
}

// ---------- Форма ----------

function updateTermVisibility() {
  termField.hidden = fields.type.value !== 'deposit';
}

function resetFormDefaults() {
  const today = todayIso();
  fields.start.value = today;
  fields.start.max = today;
  fields.type.value = 'deposit';
  fields.capitalization.value = 'monthly';
  updateTermVisibility();
}

// Тело запроса строго по контракту: те же имена полей, перечисления строками, term = null у счёта.
function readForm() {
  const type = fields.type.value;
  return {
    name: fields.name.value.trim(),
    type: type,
    sum: fields.sum.valueAsNumber,
    interestRate: fields.rate.valueAsNumber,
    startDate: fields.start.value,
    term: type === 'deposit' ? fields.term.valueAsNumber : null,
    capitalization: fields.capitalization.value
  };
}

// Те же правила, что в контракте. Это удобство для пользователя, а не защита:
// запрос можно отправить в обход страницы, поэтому сервер обязан проверять всё сам.
function validate(data) {
  const errors = [];
  if (data.name.length === 0) {
    errors.push('Укажите название.');
  }
  if (!(data.sum > 0)) {
    errors.push('Сумма должна быть больше нуля.');
  }
  if (!(data.interestRate > 0 && data.interestRate <= 100)) {
    errors.push('Ставка должна быть больше 0 и не больше 100 %.');
  }
  if (!data.startDate) {
    errors.push('Укажите дату открытия.');
  } else if (data.startDate > todayIso()) {
    errors.push('Дата открытия не может быть в будущем.');
  }
  if (data.type === 'deposit' && !(Number.isInteger(data.term) && data.term > 0)) {
    errors.push('Для вклада укажите срок: целое число месяцев больше нуля.');
  }
  return errors;
}

form.addEventListener('submit', async (event) => {
  event.preventDefault();

  const data = readForm();
  const errors = validate(data);
  if (errors.length > 0) {
    showErrors(formErrors, errors);
    return;
  }
  showErrors(formErrors, []);

  submitButton.disabled = true;
  try {
    const response = await fetch(API, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data)
    });

    if (response.status === 201) {
      const created = await response.json();
      form.reset();
      resetFormDefaults();
      showListMessage(`Добавлено: «${created.name}».`);
      await loadDeposits();
    } else if (response.status === 400) {
      const messages = await readProblemMessages(response);
      showErrors(formErrors, messages.length > 0 ? messages : ['Сервер отклонил данные. Проверьте поля формы.']);
    } else {
      showErrors(formErrors, [`Не удалось сохранить: сервер ответил ${response.status}.`]);
    }
  } catch {
    showErrors(formErrors, ['Нет связи с сервером. Данные не сохранены.']);
  } finally {
    submitButton.disabled = false;
  }
});

fields.type.addEventListener('change', updateTermVisibility);

resetFormDefaults();
loadDeposits();
checkHealth();
