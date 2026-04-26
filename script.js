const passwordInput = document.querySelector("#password");
const copyBtn = document.querySelector("#copyBtn");
const generateBtn = document.querySelector("#generateBtn");
const lengthInput = document.querySelector("#length");
const lengthValue = document.querySelector("#lengthValue");
const strengthLabel = document.querySelector("#strengthLabel");
const strengthBar = document.querySelector("#strengthBar");
const message = document.querySelector("#message");

const options = {
  uppercase: document.querySelector("#uppercase"),
  lowercase: document.querySelector("#lowercase"),
  numbers: document.querySelector("#numbers"),
  symbols: document.querySelector("#symbols")
};

const characterSets = {
  uppercase: "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
  lowercase: "abcdefghijklmnopqrstuvwxyz",
  numbers: "0123456789",
  symbols: "!@#$%&*()-_=+[]{};:,.?/"
};

function randomIndex(max) {
  const values = new Uint32Array(1);
  crypto.getRandomValues(values);
  return values[0] % max;
}

function shuffle(text) {
  const characters = [...text];

  for (let index = characters.length - 1; index > 0; index--) {
    const swapIndex = randomIndex(index + 1);
    [characters[index], characters[swapIndex]] = [characters[swapIndex], characters[index]];
  }

  return characters.join("");
}

function getSelectedSets() {
  return Object.entries(options)
    .filter(([, checkbox]) => checkbox.checked)
    .map(([key]) => characterSets[key]);
}

function calculateStrength(length, selectedCount) {
  const score = length + selectedCount * 5;

  if (score < 18) {
    return { label: "Fraca", width: "28%", color: "var(--danger)" };
  }

  if (score < 32) {
    return { label: "Boa", width: "65%", color: "var(--warn)" };
  }

  return { label: "Forte", width: "100%", color: "var(--accent)" };
}

function updateStrength() {
  const selectedSets = getSelectedSets();
  const strength = calculateStrength(Number(lengthInput.value), selectedSets.length);

  strengthLabel.textContent = strength.label;
  strengthBar.style.width = strength.width;
  strengthBar.style.background = strength.color;
}

function generatePassword() {
  const selectedSets = getSelectedSets();
  const length = Number(lengthInput.value);

  if (selectedSets.length === 0) {
    message.textContent = "Selecione pelo menos um tipo de caractere.";
    passwordInput.value = "";
    return;
  }

  let password = selectedSets.map((set) => set[randomIndex(set.length)]).join("");
  const allCharacters = selectedSets.join("");

  while (password.length < length) {
    password += allCharacters[randomIndex(allCharacters.length)];
  }

  passwordInput.value = shuffle(password.slice(0, length));
  message.textContent = "Senha gerada.";
  updateStrength();
}

async function copyPassword() {
  if (!passwordInput.value) {
    message.textContent = "Gere uma senha antes de copiar.";
    return;
  }

  try {
    await navigator.clipboard.writeText(passwordInput.value);
    message.textContent = "Senha copiada.";
  } catch {
    passwordInput.select();
    document.execCommand("copy");
    message.textContent = "Senha copiada.";
  }
}

lengthInput.addEventListener("input", () => {
  lengthValue.textContent = lengthInput.value;
  updateStrength();
  generatePassword();
});

Object.values(options).forEach((checkbox) => {
  checkbox.addEventListener("change", generatePassword);
});

generateBtn.addEventListener("click", generatePassword);
copyBtn.addEventListener("click", copyPassword);

generatePassword();
