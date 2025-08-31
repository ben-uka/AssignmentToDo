document
  .querySelector('#registerForm')
  ?.addEventListener('submit', registerUser);

async function registerUser(e) {
  e.preventDefault();
  const email = document.querySelector('#regEmail').value;
  const password = document.querySelector('#regPassword').value;
  const firstName = document.querySelector('#regFirstName').value;
  const lastName = document.querySelector('#regLastName').value;
  const response = await fetch(`${baseApiUrl}/accounts/register`, {
    method: 'POST',
    credentials: 'include',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password, firstName, lastName }),
  });
  if (response.ok) {
    alert('Användare registrerad!');
    document.querySelector('#registerForm').reset();
  } else {
    alert(
      'Misslyckades att registrera användare. Endast admin kan registrera.'
    );
  }
}
document.querySelector('#displayToDos').addEventListener('click', listToDos);
document.querySelector('#loginForm').addEventListener('submit', login);
document.querySelector('#logout').addEventListener('click', logout);
document.querySelector('#addToDoForm').addEventListener('submit', function (e) {
  e.preventDefault();
  addToDo();
});

const baseApiUrl = 'https://localhost:5001/api';

async function listToDos() {
  const response = await fetch(`${baseApiUrl}/todo`, {
    method: 'GET',
    credentials: 'include',
  });
  if (response.ok) {
    const result = await response.json();
    displayToDos(result);
  } else {
    displayError();
  }
}

async function addToDo() {
  const title = document.querySelector('#todoTitle').value;
  const content = document.querySelector('#todoContent').value;
  const response = await fetch(`${baseApiUrl}/todo`, {
    method: 'POST',
    credentials: 'include',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ title, content }),
  });
  if (response.ok) {
    listToDos();
    document.querySelector('#addToDoForm').reset();
  } else {
    displayError();
  }
}

// Prompt-baserad borttagning
async function removeToDo() {
  const id = prompt('Ange ID på ToDo att ta bort:');
  if (!id) return;
  const response = await fetch(`${baseApiUrl}/todo/${id}`, {
    method: 'DELETE',
    credentials: 'include',
  });
  if (response.ok) {
    listToDos();
  } else {
    displayError();
  }
}

async function login(e) {
  e.preventDefault();
  const email = document.querySelector('#email').value;
  const password = document.querySelector('#password').value;
  const response = await fetch(`${baseApiUrl}/accounts/login`, {
    method: 'POST',
    credentials: 'include',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password }),
  });
  if (response.ok) {
    listToDos();
    document.querySelector('#loginForm').reset();
  } else {
    displayError();
  }
}

async function logout() {
  const response = await fetch(`${baseApiUrl}/accounts/logout`, {
    method: 'POST',
    credentials: 'include',
  });
  toDoList.innerHTML = '';
}

function displayToDos(todos) {
  toDoList.innerHTML = '';
  for (let todo of todos) {
    const div = document.createElement('div');
    div.textContent = `ID: ${todo.id ?? ''} | ${todo.title ?? ''} - ${
      todo.content ?? ''
    }`;

    const deleteBtn = document.createElement('button');
    deleteBtn.textContent = 'Radera';
    deleteBtn.style.marginLeft = '1em';
    deleteBtn.onclick = () => removeToDoById(todo.id);
    div.appendChild(deleteBtn);

    const editBtn = document.createElement('button');
    editBtn.textContent = 'Redigera';
    editBtn.style.marginLeft = '0.5em';
    editBtn.onclick = () => editToDoPrompt(todo);
    div.appendChild(editBtn);

    toDoList.appendChild(div);
  }
}

async function removeToDoById(id) {
  if (!id) return;
  const response = await fetch(`${baseApiUrl}/todo/${id}`, {
    method: 'DELETE',
    credentials: 'include',
  });
  if (response.ok) {
    listToDos();
  } else {
    displayError();
  }
}

async function editToDoPrompt(todo) {
  const newTitle = prompt('Ny titel:', todo.title);
  if (newTitle === null) return;
  const newContent = prompt('Nytt innehåll:', todo.content);
  if (newContent === null) return;
  const response = await fetch(`${baseApiUrl}/todo/${todo.id}`, {
    method: 'PUT',
    credentials: 'include',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ id: todo.id, title: newTitle, content: newContent }),
  });
  if (response.ok) {
    listToDos();
  } else {
    displayError();
  }
}

document.querySelector('#removeToDo').addEventListener('click', removeToDo);

function displayError() {
  toDoList.innerHTML = '<h2 style="color:red;">UNAUTHORIZED</h2>';
}
