using System;
using System.Collections.Generic;
using System.Linq;

public partial class App : System.Windows.Application
{
	/// <summary>
	/// Časovač pro odpočítávání zbývajícího času.
	/// </summary>
	System.Windows.Threading.DispatcherTimer _countdown = new System.Windows.Threading.DispatcherTimer();

	/// <summary>
	/// Zbývající čas v [s].
	/// </summary>
	int _remainingTime;

	/// <summary>
	/// # špatných odpovědí.
	/// </summary>
	int _missed;

	/// <summary>
	/// # bodů = správných odpovědí.
	/// </summary>
	int _points;

	/// <summary>
	/// Úroveň.
	/// </summary>
	Level _level;

	/// <summary>
	/// GUI.
	/// </summary>
	MainWindow _wnd = null;

	/// <summary>
	/// Aktuální zadání.
	/// </summary>
	Exercise _exercise = null;

	public App()
	{
		_countdown.Interval = new TimeSpan(0, 0, 1);
		_countdown.Tick += _Tick;
	}

	/// <summary>
	/// Vytvoření a zobrazení GUI.
	/// </summary>
	private void _Start(object sender, System.Windows.StartupEventArgs e)
	{
		_wnd = new MainWindow(this);
		_wnd.RemainingTimeChanged(_remainingTime);
		_wnd.Show();
	}

	public enum Level
	{
		/// <summary>
		/// Obtížnost pro studenta 1. ročníku konzervatoře.
		/// </summary>
		Beginner,

		/// <summary>
		/// Obtížnost pro studenta 2. ročníku konzervatoře.
		/// </summary>
		Advanced
	}

	/// <summary>
	/// Spuštění hry.
	/// </summary>
	/// <param name="level">požadovaná úroveň</param>
	public void Begin(Level level)
	{
		Console.Clear();
		_level = level;
		_remainingTime = 300;
		_wnd.RemainingTimeChanged(_remainingTime);
		_missed = 0;  // inicializace počítadla chyb
		_points = 0;  // inicializace počítadla bodů (správných odpovědí)
		_wnd.PointsChanged(_points);
		_countdown.Start();  // spuštění časomíry

		_NewTask();
	}

	/// <summary>
	/// Zadání nové úlohy.
	/// </summary>
	private void _NewTask()
	{
		Exercise ex = new Exercise(_level);
		while (ex.Solution.Any(note => Math.Abs(note.Accidental) > 2)) {
			// řešení obsahuje notu s více než dvěma posuvkami (tedy v praxi neužívanou
			// a nezadatelnou do GUI), vygenerujeme jiné cvičení
			ex = new Exercise(_level);
		};

		_wnd.ExcerciseChanged(ex.Task().Replace('|', '\n'));
		_exercise = ex;
	}

	/// <summary>
	/// Kontrola odpovědi.
	/// </summary>
	/// <param name="notes">Uživatelem zadané řešení.</param>
	public void Evaluate(List<Note> notes)
	{
		var solution = _exercise.Solution;
		bool passed = (solution.Count == notes.Count);  // kontrola, že byl zadán správný počet not
		if (passed) {
			// výpočet rozdílu oktáv očekávaného a odevzdaného akordu
			int diff = solution[0].Octave - notes[0].Octave;
			for (int i = 0; i < solution.Count; ++i) {
				Note expected = solution[i];
				Note actual = notes[i];
				if (expected.Basename != actual.Basename ||
						expected.Accidental != actual.Accidental ||
						expected.Octave - actual.Octave != diff) {
					passed = false;
					break;
				}
			}
		}

		if (passed) {
			++_points;
			_wnd.PointsChanged(_points);
		}
		else {
			++_missed;
		}

		Console.WriteLine(
@"============================================================
Zadání: {0}

Správné řešení:   {1}
Odesláno:         {2}

{3} -> celkově {4} správně ({5} špatně)",
			_exercise.Task().Replace('|', ' '),
			String.Join(" ", solution),
			String.Join(" ", notes),
			passed ? "SPRÁVNĚ" : "ŠPATNĚ", _points, _missed);

		_NewTask();
	}

	/// <summary>
	/// Ukončení hry.
	/// </summary>
	public void TerminateGame()
	{
		_countdown.Stop();
		_wnd.GameOver(_points);
	}

	private void _Tick(object sender, EventArgs e)
	{
		--_remainingTime;
		_wnd.RemainingTimeChanged(_remainingTime);
		if (_remainingTime == 0) {
			TerminateGame();
		}
	}
}
