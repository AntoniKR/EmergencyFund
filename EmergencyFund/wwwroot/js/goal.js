// Страница цели. Накопленную сумму считает сервер из вкладов при каждом GET,
// поэтому страница всегда показывает актуальный прогресс, даже если вклады меняли на другой странице.
const GOAL_API = '/api/goal';

const goalSummary = document.getElementById('goal-summary');
const goalForm = document.getElementById('goal-form');
const goalFields = {
  expenses: document.getElementById('goal-expenses'),
  months: document.getElementById('goal-months'),
  savings: document.getElementById('goal-savings')
};
const goalErrors = document.getElementById('goal-errors');
const goalSubmit = document.getElementById('goal-submit');
const goalDelete = document.getElementById('goal-delete');

async function loadGoal() {
  try {
    const response = await fetch(GOAL_API);
    if (response.status === 404) {
      renderGoal(null);
      return;
    }
    if (!response.ok) {
      goalSummary.replaceChildren(el('p', 'goal-empty', `Цель не загрузилась: сервер ответил ${response.status}.`));
      return;
    }
    const goal = await response.json();
    renderGoal(goal);
    fillForm(goal);
  } catch {
    goalSummary.replaceChildren(el('p', 'goal-empty', 'Нет связи с сервером.'));
  }
}

function describeMonthsToGoal(goal) {
  if (goal.monthsToGoal === 0) {
    return 'Цель достигнута';
  }
  if (goal.monthsToGoal === null) {
    return 'Не посчитать: вы ничего не откладываете';
  }
  return `${goal.monthsToGoal} ${plural(goal.monthsToGoal, ['месяц', 'месяца', 'месяцев'])} при текущих накоплениях`;
}

// 404 от сервера = цели нет. Тогда goal === null.
function renderGoal(goal) {
  goalSummary.replaceChildren();

  if (goal === null) {
    goalSummary.append(el('p', 'goal-empty', 'Цель ещё не задана. Укажите расходы в месяц и на сколько месяцев нужен запас — посчитаю, сколько уже собрано.'));
    goalSubmit.textContent = 'Задать цель';
    goalDelete.hidden = true;
    return;
  }

  const line = el('p', 'goal-line');
  line.append(
    el('span', 'goal-percent', `${percent.format(goal.progressPercent)} %`),
    document.createTextNode(` от цели ${money.format(goal.targetAmount)}`)
  );

  // Больше 100 % бывает: полоса заполняется до конца, а число показывает реальный процент.
  const filled = Math.min(goal.progressPercent, 100);
  const meter = el('div', 'meter');
  meter.setAttribute('role', 'progressbar');
  meter.setAttribute('aria-valuemin', '0');
  meter.setAttribute('aria-valuemax', '100');
  meter.setAttribute('aria-valuenow', String(filled));
  meter.setAttribute('aria-valuetext', `${percent.format(goal.progressPercent)} % от цели`);
  const fill = el('div', 'meter-fill');
  fill.style.width = `${filled}%`;
  meter.append(fill);

  const figures = el('dl', 'figures');
  addFigure(figures, 'Собрано', money.format(goal.currentAmount));
  addFigure(figures, 'Осталось', money.format(goal.remainingAmount));
  addFigure(figures, 'До цели', describeMonthsToGoal(goal));

  goalSummary.append(line, meter, figures);
  goalSubmit.textContent = 'Сохранить цель';
  goalDelete.hidden = false;
}

function fillForm(goal) {
  goalFields.expenses.value = goal.monthlyExpenses;
  goalFields.months.value = goal.months;
  goalFields.savings.value = goal.monthlySavings;
}

// Правила из контракта. Сервер пока проверяет только наличие полей (правила «больше 0» — день 4),
// поэтому без этой проверки months = 0 дал бы 500 от деления на ноль.
function validateGoal(data) {
  const errors = [];
  if (!(data.monthlyExpenses > 0)) {
    errors.push('Расходы в месяц должны быть больше нуля.');
  }
  if (!(Number.isInteger(data.months) && data.months > 0)) {
    errors.push('Число месяцев — целое и больше нуля.');
  }
  if (!(data.monthlySavings >= 0)) {
    errors.push('Сколько откладываете — 0 или больше.');
  }
  return errors;
}

goalForm.addEventListener('submit', async (event) => {
  event.preventDefault();

  const data = {
    monthlyExpenses: goalFields.expenses.valueAsNumber,
    months: goalFields.months.valueAsNumber,
    monthlySavings: goalFields.savings.valueAsNumber
  };
  const errors = validateGoal(data);
  if (errors.length > 0) {
    showErrors(goalErrors, errors);
    return;
  }
  showErrors(goalErrors, []);

  goalSubmit.disabled = true;
  try {
    const response = await fetch(GOAL_API, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data)
    });

    if (response.status === 200) {
      renderGoal(await response.json());
    } else if (response.status === 400) {
      const messages = await readProblemMessages(response);
      showErrors(goalErrors, messages.length > 0 ? messages : ['Сервер отклонил данные. Проверьте поля цели.']);
    } else {
      showErrors(goalErrors, [`Не удалось сохранить цель: сервер ответил ${response.status}.`]);
    }
  } catch {
    showErrors(goalErrors, ['Нет связи с сервером. Цель не сохранена.']);
  } finally {
    goalSubmit.disabled = false;
  }
});

goalDelete.addEventListener('click', async () => {
  if (!confirm('Удалить цель? Вклады останутся.')) {
    return;
  }
  try {
    const response = await fetch(GOAL_API, { method: 'DELETE' });
    if (response.status === 204 || response.status === 404) {
      goalForm.reset();
      showErrors(goalErrors, []);
      renderGoal(null);
    } else {
      showErrors(goalErrors, [`Не удалось удалить цель: сервер ответил ${response.status}.`]);
    }
  } catch {
    showErrors(goalErrors, ['Нет связи с сервером. Цель не удалена.']);
  }
});

loadGoal();
checkHealth();
