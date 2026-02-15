const catalog = {
  movies: [
    {
      id: "m1",
      title: "Dune",
      year: 2021,
      genre: "Sci-Fi",
      poster: "https://images.unsplash.com/photo-1519608487953-e999c86e7455?w=900&auto=format&fit=crop",
      embedUrl: "https://www.youtube.com/embed/n9xhJrPXop4"
    },
    {
      id: "m2",
      title: "Top Gun: Maverick",
      year: 2022,
      genre: "Action",
      poster: "https://images.unsplash.com/photo-1508615070457-7baeba4003ab?w=900&auto=format&fit=crop",
      embedUrl: "https://www.youtube.com/embed/giXco2jaZ_4",
      watchUrl: "https://drive.google.com"
    }
  ],
  series: [
    {
      id: "s1",
      title: "The Last of Us",
      year: 2023,
      genre: "Drama",
      poster: "https://images.unsplash.com/photo-1524985069026-dd778a71c7b4?w=900&auto=format&fit=crop",
      episodes: [
        { label: "Episode 1", embedUrl: "https://www.youtube.com/embed/uLtkt8BonwM" },
        { label: "Episode 2", embedUrl: "https://www.youtube.com/embed/rBRRDpQ0yc0" }
      ]
    }
  ]
};

const state = {
  tab: "movies",
  query: "",
  genre: "all",
  currentItem: null,
  currentEpisode: 0
};

const grid = document.getElementById("cardsGrid");
const toast = document.getElementById("toast");
const searchInput = document.getElementById("searchInput");
const genreFilter = document.getElementById("genreFilter");
const modal = document.getElementById("playerModal");
const frame = document.getElementById("playerFrame");
const modalTitle = document.getElementById("modalTitle");
const episodePanel = document.getElementById("episodePanel");
const openExternalBtn = document.getElementById("openExternal");

function init() {
  wireEvents();
  hydrateGenres();
  render();
}

function wireEvents() {
  document.querySelectorAll(".tab").forEach(tab => {
    tab.addEventListener("click", () => {
      state.tab = tab.dataset.tab;
      state.genre = "all";
      state.query = "";
      searchInput.value = "";
      document.querySelectorAll(".tab").forEach(t => t.classList.toggle("active", t === tab));
      hydrateGenres();
      render();
      notify(`Switched to ${state.tab}`);
    });
  });

  searchInput.addEventListener("input", (e) => {
    state.query = e.target.value.trim().toLowerCase();
    render();
  });

  genreFilter.addEventListener("change", (e) => {
    state.genre = e.target.value;
    render();
  });

  modal.addEventListener("close", () => {
    frame.src = "about:blank";
  });

  openExternalBtn.addEventListener("click", () => {
    if (!state.currentItem) return;
    const externalUrl = state.currentItem.watchUrl || getActiveEmbedUrl();
    window.open(externalUrl, "_blank", "noopener");
  });
}

function hydrateGenres() {
  const genres = [...new Set(catalog[state.tab].map(item => item.genre))].sort();
  genreFilter.innerHTML = `<option value="all">All genres</option>${genres.map(g => `<option value="${g}">${g}</option>`).join("")}`;
}

function getFiltered() {
  return catalog[state.tab].filter(item => {
    const haystack = `${item.title} ${item.genre} ${item.year}`.toLowerCase();
    const queryMatch = !state.query || haystack.includes(state.query);
    const genreMatch = state.genre === "all" || item.genre === state.genre;
    return queryMatch && genreMatch;
  });
}

function render() {
  const items = getFiltered();
  grid.innerHTML = items.map(item => `
    <article class="card" data-id="${item.id}" tabindex="0" aria-label="${item.title}">
      <img class="poster" src="${item.poster}" alt="${item.title} poster" loading="lazy" />
      <div class="card-body">
        <h3>${item.title}</h3>
        <p class="meta">${item.genre} · ${item.year}</p>
      </div>
    </article>
  `).join("");

  grid.querySelectorAll(".card").forEach(card => {
    card.addEventListener("click", () => openPlayer(card.dataset.id));
    card.addEventListener("keydown", (e) => {
      if (e.key === "Enter" || e.key === " ") {
        e.preventDefault();
        openPlayer(card.dataset.id);
      }
    });
  });
}

function openPlayer(id) {
  const item = catalog[state.tab].find(entry => entry.id === id);
  if (!item) return;

  state.currentItem = item;
  state.currentEpisode = 0;
  modalTitle.textContent = item.title;

  if (item.episodes?.length) {
    renderEpisodes(item);
  } else {
    episodePanel.innerHTML = "";
  }

  frame.src = getActiveEmbedUrl();
  modal.showModal();
  notify(`Playing ${item.title}`);
}

function renderEpisodes(series) {
  episodePanel.innerHTML = series.episodes.map((ep, i) =>
    `<button class="chip ${i === state.currentEpisode ? "active" : ""}" data-index="${i}">${ep.label}</button>`
  ).join("");

  episodePanel.querySelectorAll(".chip").forEach(chip => {
    chip.addEventListener("click", () => {
      state.currentEpisode = Number(chip.dataset.index);
      frame.src = getActiveEmbedUrl();
      renderEpisodes(series);
    });
  });
}

function getActiveEmbedUrl() {
  if (!state.currentItem) return "about:blank";
  if (state.currentItem.episodes?.length) {
    return state.currentItem.episodes[state.currentEpisode].embedUrl;
  }
  return state.currentItem.embedUrl;
}

let toastTimer = null;
function notify(text) {
  toast.textContent = text;
  toast.classList.add("show");
  clearTimeout(toastTimer);
  toastTimer = setTimeout(() => toast.classList.remove("show"), 1700);
}

init();
