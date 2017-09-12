using System;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Třída reprezentující úkol.
/// </summary>
class Exercise
{
	static Random _random = new Random();

	/// <summary>
	/// Zadaný akord (např. zvětšeně zvětšený septakord).
	/// </summary>
	private Akkord _akkord;

	/// <summary>
	/// Počáteční nota zadaného akordu (ostatní noty správného řešení jsou již jasně určeny).
	/// </summary>
	private Note _start;

	/// <summary>
	/// Vygenerování náhodného zadání dle zvolené obtížnosti.
	/// </summary>
	public Exercise(App.Level level)
	{
		if (_random.Next(level == App.Level.Beginner ? 1 : 2) == 0) {  // losování kvintakord / septakord
			// generuje se kvintakkord
			var values = Enum.GetValues(typeof(Kvintakkord.Type));
			var type = (Kvintakkord.Type) values.GetValue(_random.Next(values.Length));  // losování dur, moll apod.
			int umkehrung = _random.Next(Kvintakkord.Count);  // losování obratu
			_akkord = new Kvintakkord(type, umkehrung);
		}
		else {
			// generuje se septakkord
			var values = Enum.GetValues(typeof(Septakkord.Type));
			var type = (Septakkord.Type) values.GetValue(_random.Next(values.Length));
			int umkehrung = _random.Next(Septakkord.Count);
			_akkord = new Septakkord(type, umkehrung);
		}

		var basenames = new string[] { "c", "d", "e", "f", "g", "a", "h" };
		_start = new Note(
			basenames[_random.Next(basenames.Length)],  // vylosování náhodného kořene noty
			0,  // nastavení oktávy
			_random.Next(-1, 2));  // vylosování posuvky – pouze 1 béčko / bez posuvky / 1 křížek
	}

	public override string ToString()
	{
		return String.Format(Task());
	}
	
	/// <summary>
	/// Slovní zadání úkolu, např. "zmenšený kvintakord od c".
	/// </summary>
	public string Task()
	{
		return String.Format("{0}|od {1}", _akkord.Task(), _start.Name); 
	}

	List<Note> _solution = null;

	/// <summary>
	/// Vyřešení zadání.
	/// </summary>
	public List<Note> Solution
	{
		get
		{
			if (_solution != null) {
				return _solution;
			}

			var comparer = new Note.BaseComparer();
			var solution = new List<Note>();

			solution.Add((Note) _start.Clone());
			for (int i = _akkord.Umkehrung; i < _akkord.Shifts.Count; ++i) {
				var args = _akkord.Shifts[i];
				solution.Add(solution.Last().Shift(args.Item1, args.Item2));
			}
			for (int i = _akkord.Umkehrung - 1; i >= 0; --i) {
				var args = _akkord.Shifts[i];
				var note = solution.First().Shift(-args.Item1, -args.Item2);
				while (comparer.Compare(note, _start) < 0) {
					++note.Octave;
				}
				solution.Insert(0, note);
			}
			solution.Sort(comparer);

			_solution = solution;
			return solution;
		}
	}
}
