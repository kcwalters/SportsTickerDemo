(function () {
  var STORAGE_KEY = 'sportsTickerSelections';
  var SECTION_STATE_KEY = 'tickerControlsExpanded';
  var NFL_HEADER_LOGO = './images/nfl.svg';
  var DEFAULT_HEADER_LOGO = './img/AllLeaguesFramed.jpg';

  function loadSelections() {
    try {
      var raw = localStorage.getItem(STORAGE_KEY);
      return raw ? JSON.parse(raw) : null;
    } catch (e) {
      return null;
    }
  }

  function saveSelections(checkboxes) {
    var selections = {};
    checkboxes.forEach(function (cb) { selections[cb.id] = !!cb.checked; });
    try { localStorage.setItem(STORAGE_KEY, JSON.stringify(selections)); } catch (e) {}
  }

  function updateHeaderLogo(checkboxes) {
    var headerImg = document.getElementById('header-logo');
    if (!headerImg) return;
    var nflChecked = checkboxes.some(function (cb) { return cb.id === 'cb-nfl' && cb.checked; });
    headerImg.src = nflChecked ? NFL_HEADER_LOGO : DEFAULT_HEADER_LOGO;
  }

  function updateVisibilityByState(checkboxes) {
    checkboxes.forEach(function (cb) {
      var targetId = cb.getAttribute('data-target');
      var el = document.getElementById(targetId);
      if (!el) return;
      el.style.display = cb.checked ? '' : 'none';
    });
    updateHeaderLogo(checkboxes);
  }

  function initControls() {
    var checkboxes = Array.prototype.slice.call(document.querySelectorAll('#ticker-controls input[type="checkbox"]'));

    var saved = loadSelections();
    if (saved) {
      checkboxes.forEach(function (cb) {
        if (Object.prototype.hasOwnProperty.call(saved, cb.id)) {
          cb.checked = !!saved[cb.id];
        }
      });
    }

    var checked = checkboxes.filter(function (cb) { return cb.checked; });
    if (checked.length > 1) { checked.slice(1).forEach(function (cb) { cb.checked = false; }); }
    if (checked.length === 0 && checkboxes.length) { checkboxes[0].checked = true; }

    updateVisibilityByState(checkboxes);

    checkboxes.forEach(function (cb) {
      cb.addEventListener('change', function () {
        if (cb.checked) {
          checkboxes.forEach(function (other) { if (other !== cb) other.checked = false; });
        } else {
          var anyChecked = checkboxes.some(function (x) { return x.checked; });
          if (!anyChecked) cb.checked = true;
        }
        updateVisibilityByState(checkboxes);
        saveSelections(checkboxes);
      });
    });

    var details = document.getElementById('ticker-controls-details');
    if (details) {
      try {
        var expandedRaw = localStorage.getItem(SECTION_STATE_KEY);
        if (expandedRaw !== null) { details.open = expandedRaw === 'true'; }
        details.addEventListener('toggle', function () {
          localStorage.setItem(SECTION_STATE_KEY, details.open ? 'true' : 'false');
        });
      } catch (e) {}
    }
  }

  document.addEventListener('DOMContentLoaded', initControls);
})();
