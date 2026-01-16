// Sports data
const sportsData = {
    soccer: [
        { teams: 'Manchester United vs Liverpool', score: '2 - 1', time: 'FT', icon: '⚽' },
        { teams: 'Barcelona vs Real Madrid', score: '3 - 3', time: '78\'', icon: '⚽' },
        { teams: 'PSG vs Bayern Munich', score: '1 - 2', time: 'FT', icon: '⚽' },
        { teams: 'Chelsea vs Arsenal', score: '0 - 0', time: '45\'', icon: '⚽' }
    ],
    basketball: [
        { teams: 'Lakers vs Warriors', score: '108 - 112', time: 'Q4 2:30', icon: '🏀' },
        { teams: 'Celtics vs Heat', score: '95 - 88', time: 'Final', icon: '🏀' },
        { teams: 'Nets vs Bucks', score: '102 - 98', time: 'Final', icon: '🏀' },
        { teams: 'Suns vs Mavericks', score: '87 - 91', time: 'Q3 8:15', icon: '🏀' }
    ],
    football: [
        { teams: 'Patriots vs Bills', score: '21 - 17', time: 'Q3 5:42', icon: '🏈' },
        { teams: 'Cowboys vs Eagles', score: '28 - 24', time: 'Final', icon: '🏈' },
        { teams: 'Chiefs vs Broncos', score: '35 - 10', time: 'Final', icon: '🏈' },
        { teams: 'Packers vs Bears', score: '14 - 14', time: 'Q2 0:45', icon: '🏈' }
    ],
    baseball: [
        { teams: 'Yankees vs Red Sox', score: '4 - 2', time: 'Top 7th', icon: '⚾' },
        { teams: 'Dodgers vs Giants', score: '3 - 5', time: 'Final', icon: '⚾' },
        { teams: 'Cubs vs Cardinals', score: '2 - 1', time: 'Bot 5th', icon: '⚾' },
        { teams: 'Astros vs Rangers', score: '6 - 3', time: 'Final', icon: '⚾' }
    ]
};

// Initialize ticker
let isPaused = false;
let isFast = false;

function generateTickerContent() {
    const ticker = document.getElementById('ticker');
    let content = '';
    
    // Combine all sports data for the ticker
    Object.keys(sportsData).forEach(sport => {
        sportsData[sport].forEach(game => {
            content += `
                <span class="ticker-item">
                    <span class="sport-icon">${game.icon}</span>
                    <span class="teams">${game.teams}</span> - 
                    <span class="score">${game.score}</span>
                    <span class="time">(${game.time})</span>
                </span>
            `;
        });
    });
    
    // Duplicate content for seamless loop
    ticker.innerHTML = content + content;
}

function populateSportsCards() {
    Object.keys(sportsData).forEach(sport => {
        const container = document.getElementById(`${sport}-scores`);
        let html = '';
        
        sportsData[sport].forEach(game => {
            html += `
                <div class="score-item">
                    <div class="teams">${game.teams}</div>
                    <div class="result">${game.score}</div>
                    <div class="time">${game.time}</div>
                </div>
            `;
        });
        
        container.innerHTML = html;
    });
}

// Control buttons
document.getElementById('pauseBtn').addEventListener('click', function() {
    const ticker = document.getElementById('ticker');
    isPaused = !isPaused;
    
    if (isPaused) {
        ticker.classList.add('paused');
        this.innerHTML = '▶ Play';
    } else {
        ticker.classList.remove('paused');
        this.innerHTML = '⏸ Pause';
    }
});

document.getElementById('speedBtn').addEventListener('click', function() {
    const ticker = document.getElementById('ticker');
    isFast = !isFast;
    
    if (isFast) {
        ticker.classList.add('fast');
        this.innerHTML = '🐌 Slow';
    } else {
        ticker.classList.remove('fast');
        this.innerHTML = '⚡ Speed';
    }
});

document.getElementById('refreshBtn').addEventListener('click', function() {
    // Simulate refreshing data
    this.innerHTML = '🔄 Refreshing...';
    this.disabled = true;
    
    setTimeout(() => {
        // In a real app, this would fetch new data from an API
        generateTickerContent();
        populateSportsCards();
        this.innerHTML = '🔄 Refresh';
        this.disabled = false;
    }, 1000);
});

// Initialize on page load
window.addEventListener('DOMContentLoaded', () => {
    generateTickerContent();
    populateSportsCards();
});
