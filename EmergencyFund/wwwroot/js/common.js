// Общие функции для обеих страниц. Подключается раньше скрипта страницы.

const money = new Intl.NumberFormat('ru-RU', { style: 'currency', currency: 'RUB' });
const percent = new Intl.NumberFormat('ru-RU', { maximumFractionDigits: 2 });

// API отдаёт дату строкой "YYYY-MM-DD". new Date("2026-11-03") считает её полуночью по UTC,
// и западнее Гринвича дата съехала бы на день назад. Поэтому разбираем строку вручную.
function formatDate(isoDate) {
  const [year, month, day] = isoDate.split('-');
  return `${day}.${month}.${year}`;
}

// Сегодня по местному времени в формате "YYYY-MM-DD".
// toISOString() не подходит: он переводит в UTC, и в первые часы после полуночи по Москве вернёт вчера.
function todayIso() {
  const now = new Date();
  const month = String(now.getMonth() + 1).padStart(2, '0');
  const day = String(now.getDate()).padStart(2, '0');
  return `${now.getFullYear()}-${month}-${day}`;
}

// Русские окончания: 1 месяц, 2 месяца, 5 месяцев.
function plural(count, forms) {
  const mod10 = count % 10;
  const mod100 = count % 100;
  if (mod10 === 1 && mod100 !== 11) {
    return forms[0];
  }
  if (mod10 >= 2 && mod10 <= 4 && (mod100 < 12 || mod100 > 14)) {
    return forms[1];
  }
  return forms[2];
}

// Создаёт элемент и кладёт текст через textContent, а не innerHTML.
// Название вклада вводит пользователь: через innerHTML строка вида <img onerror=...> выполнилась бы как код (XSS).
function el(tag, className, text) {
  const node = document.createElement(tag);
  if (className) {
    node.className = className;
  }
  if (text !== undefined) {
    node.textContent = text;
  }
  return node;
}

// Пара «подпись — значение» в списке <dl>. Значение — строка или готовый элемент.
function addFigure(list, label, value) {
  const wrapper = el('div');
  const description = el('dd');
  if (typeof value === 'string') {
    description.textContent = value;
  } else {
    description.append(value);
  }
  wrapper.append(el('dt', null, label), description);
  list.append(wrapper);
}

// Показывает список ошибок в блоке под формой; пустой список прячет блок.
function showErrors(container, messages) {
  container.replaceChildren();
  if (messages.length === 0) {
    container.hidden = true;
    return;
  }
  const list = el('ul');
  for (const message of messages) {
    list.append(el('li', null, message));
  }
  container.append(list);
  container.hidden = false;
}

// На 400 ASP.NET Core отвечает ProblemDetails: { title, errors: { "Поле": ["сообщение"] } }.
async function readProblemMessages(response) {
  try {
    const problem = await response.json();
    if (problem.errors) {
      return Object.values(problem.errors).flat();
    }
    if (problem.title) {
      return [problem.title];
    }
  } catch {
    // тело не JSON — покажем общее сообщение
  }
  return [];
}

async function checkHealth() {
  const status = document.getElementById('api-status');
  try {
    const response = await fetch('/api/health');
    const data = await response.json();
    status.textContent = data.status === 'ok' ? 'Сервер на связи.' : 'Сервер ответил, но статус не «ok».';
  } catch {
    status.textContent = 'Сервер не отвечает.';
  }
}
