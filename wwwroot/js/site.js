document.addEventListener('DOMContentLoaded', () => {
  document.querySelectorAll('[data-toggle-password]').forEach(button => {
    button.addEventListener('click', () => {
      const input = button.parentElement.querySelector('input');
      input.type = input.type === 'password' ? 'text' : 'password';
      button.setAttribute('aria-label', input.type === 'password' ? 'Show password' : 'Hide password');
    });
  });
  document.querySelectorAll('[data-open]').forEach(button => {
    button.addEventListener('click', () => document.getElementById(button.dataset.open)?.classList.add('open'));
  });
  document.querySelectorAll('[data-close]').forEach(button => {
    button.addEventListener('click', () => button.closest('.drawer-backdrop')?.classList.remove('open'));
  });
  document.querySelectorAll('.drawer-backdrop').forEach(backdrop => {
    backdrop.addEventListener('click', e => { if (e.target === backdrop) backdrop.classList.remove('open'); });
  });
  const search = document.getElementById('patientSearch');
  search?.addEventListener('input', () => {
    const query = search.value.toLowerCase().trim();
    document.querySelectorAll('.patient-result').forEach(item => {
      item.style.display = item.dataset.search.includes(query) ? '' : 'none';
    });
  });
  const date = document.getElementById('calendarDate');
  date?.addEventListener('change', () => { if (date.value) window.location.href = `?date=${date.value}`; });
});
