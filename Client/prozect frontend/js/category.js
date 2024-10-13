const list = document.querySelector('.list');
const items = document.querySelectorAll('.blocks_item');

function filterItems(targetId) {
  items.forEach(item => {
    item.style.display = 'none';
    if (targetId === '0' || item.classList.contains(`ctg${targetId}`)) {
      item.style.display = 'block';
    }
  });

  const filteredItems = document.querySelectorAll('.blocks_item:visible');
  const rows = [];

  for (let i = 0; i < filteredItems.length; i += 4) {
    rows.push(filteredItems.slice(i, i + 4));
  }

  const blocks = document.querySelectorAll('.block');
  blocks.forEach(block => block.innerHTML = '');

  rows.forEach(row => {
    const rowElement = document.createElement('div');
    rowElement.className = 'blocks_row';
    row.forEach(item => {
      rowElement.appendChild(item.cloneNode(true));
    });
    blocks[0].appendChild(rowElement);
  });
}

list.addEventListener('click', event => {
  const targetId = event.target.dataset.id;
  filterItems(targetId);
});

const listItems = document.querySelectorAll('.list_item');

listItems.forEach(item => {
  item.addEventListener('click', () => {
    listItems.forEach(item => item.classList.remove('active'));
    item.classList.add('active');
  });
});