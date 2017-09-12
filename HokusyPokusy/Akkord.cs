using System;
using System.Collections.Generic;

/// <summary>
/// Reprezentace akordu.
/// </summary>
public abstract class Akkord 
{
	public Akkord()
	{
		Shifts = new List<Tuple<int, int>>();
	}

	/// <summary>
	/// Mezery mezi notami v notové a půltónové vzdálenosti.
	/// </summary>
	public List<Tuple<int, int>> Shifts 
	{
		get;
	}

	/// <summary>
	/// Obrat akordu.
	/// </summary>
	public int Umkehrung
	{
		get;
		protected set;
	}

	/// <summary>
	/// Slovní popis akordu, jednotlivé části jsou odděleny pomocí |.
	/// </summary>
	/// <example>zvětšeně velký|septakord|od g</example>
	public abstract string Task();
}

public class Kvintakkord : Akkord
{
	public enum Type
	{
		/// <summary>
		/// dur
		/// </summary>
		Major,

		/// <summary>
		/// moll
		/// </summary>
		Minor,

		/// <summary>
		/// zvětšený
		/// </summary>
		Augmented,

		/// <summary>
		/// zmenšený
		/// </summary>
		Diminished
	}

	/// <summary>
	/// Typ tohoto akordu.
	/// </summary>
	Type _type;

	/// <summary>
	/// Počet not v kvintakordu.
	/// </summary>
	public const int Count = 3;

	public Kvintakkord(Type type, int umkehrung)
	{
		if (umkehrung >= Count) {  // obratů je jen tolik, kolik je not v akordu
			throw new ArgumentException();
		}

		_type = type;
		Umkehrung = umkehrung;

		int first = 3, second = 3;  // počty půltónů v terciích daného kvintakordu (3 – malá, 4 – velká)
		if (type == Type.Major || type == Type.Augmented) {  // jedná-li se o durový nebo zvětšený kvintakord,
			first = 4;  // první tercie je velká
		}
		if (type == Type.Minor || type == Type.Augmented) {  // jedná-li se o mollový nebo zvětšený kvintakord
			second = 4;  // druhá tercie je velká
		}

		Shifts.Add(Tuple.Create(2, first));   // druhá nota akordu je ve vzdálenosti dvou not od první noty
		Shifts.Add(Tuple.Create(2, second));  // třetí nota akordu je ve vzdálenosti dvou not od druhé noty
	}

	public override string Task()
	{
		string result = null;
		switch (_type) {
		case Type.Augmented:
			result = "zvětšený";
			break;
		case Type.Diminished:
			result = "zmenšený";
			break;
		case Type.Major:
			result = "durový";
			break;
		case Type.Minor:
			result = "mollový";
			break;
		}
		result += '|';
		switch (Umkehrung) {
		case 0:
			result += "kvintakord";
			break;
		case 1:
			result += "sextakord";
			break;
		case 2:
			result += "kvartsextakord";
			break;
		}

		return result;
	}
}

public class Septakkord : Akkord
{
	public enum Type
	{
		/// <summary>
		/// tvrdě velký
		/// </summary>
		Major,

		/// <summary>
		/// měkce velký
		/// </summary>
		Minor,

		/// <summary>
		/// tvrdě malý (dominantní)
		/// </summary>
		Dominant,

		/// <summary>
		/// zmenšeně zmenšený
		/// </summary>
		Diminished,

		/// <summary>
		/// zmenšeně malý
		/// </summary>
		HalfDiminished,

		/// <summary>
		/// měkce malý
		/// </summary>
		MinorMajor,

		/// <summary>
		/// zvětšeně velký
		/// </summary>
		AugmentedMajor
	}

	/// <summary>
	/// Typ tohoto akordu.
	/// </summary>
	Type _type;

	/// <summary>
	/// Počet not v septakordu.
	/// </summary>
	public const int Count = 4;

	public Septakkord(Type type, int umkehrung)
	{
		if (umkehrung >= Count) {
			throw new ArgumentException();
		}

		_type = type;
		Umkehrung = umkehrung;

		int first = 3, second = 3, third = 3;
		if (type == Type.Major || type == Type.Dominant || type == Type.AugmentedMajor) {
			first = 4;
		}
		if (type == Type.Minor || type == Type.MinorMajor || type == Type.AugmentedMajor) {
			second = 4;
		}
		if (type == Type.Major || type == Type.MinorMajor) {
			third = 4;
		}

		Shifts.Add(Tuple.Create(2, first));
		Shifts.Add(Tuple.Create(2, second));
		Shifts.Add(Tuple.Create(2, third));
	}

	public override string Task()
	{
		string result = null;
		switch (_type) {
		case Type.Major:
			result = "tvrdě velký";
			break;
		case Type.Minor:
			result = "měkce malý";
			break;
		case Type.Dominant:
			result = "tvrdě malý";
			break;
		case Type.Diminished:
			result = "zmenšeně zmenšený";
			break;
		case Type.HalfDiminished:
			result = "zmenšeně malý";
			break;
		case Type.MinorMajor:
			result = "měkce velký";
			break;
		case Type.AugmentedMajor:
			result = "zvětšeně velký";
			break;
		}
		result += '|';
		switch (Umkehrung) {
		case 0:
			result += "septakord";
			break;
		case 1:
			result += "kvintsextakord";
			break;
		case 2:
			result += "terckvartakord";
			break;
		case 3:
			result += "sekundakord";
			break;
		}

		return result;
	}
}
