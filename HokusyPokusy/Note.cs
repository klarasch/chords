using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Reprezentace noty.
/// </summary>
public class Note : System.ICloneable
{
	class _ScaleItem
	{
		/// <summary>
		/// Kořen názvu (c, d, e, ...).
		/// </summary>
		public string Basename;

		/// <summary>
		/// Počet půltónů k další notě ve stupnici.
		/// </summary>
		public int HalftonesNumber;
	}

	/// <summary>
	/// Řada not (definovaná počtem půltónů mezi jednotlivými notami).
	/// </summary>
	static List<_ScaleItem> _scale;

	/// <summary>
	/// Základní řada not a počty půltónů mezi nimi.
	/// </summary>
	static Note()
	{
		_scale = new List<_ScaleItem>() { 
			new _ScaleItem() { Basename = "c", HalftonesNumber = 2 },
			new _ScaleItem() { Basename = "d", HalftonesNumber = 2 },
			new _ScaleItem() { Basename = "e", HalftonesNumber = 1 },
			new _ScaleItem() { Basename = "f", HalftonesNumber = 2 },
			new _ScaleItem() { Basename = "g", HalftonesNumber = 2 },
			new _ScaleItem() { Basename = "a", HalftonesNumber = 2 },
			new _ScaleItem() { Basename = "h", HalftonesNumber = 1 }
		};
	}

	/// <summary>
	/// Číslo kořene noty (c = 1, d = 2, ...).
	/// </summary>
	private int _basenumber;

	/// <summary>
	/// Porovnání 1. podle oktávy a 2. podle kořenu noty.
	/// </summary>
	public class BaseComparer : IComparer<Note>
	{
		public int Compare(Note n1, Note n2)
		{
			if (n1.Octave.CompareTo(n2.Octave) != 0) {
				return n1.Octave.CompareTo(n2.Octave);
			}
			else {
				return n1._basenumber.CompareTo(n2._basenumber); 
			}
		}
	}

	/// <summary>
	/// Název kořene noty [c–h].
	/// </summary>
	public string Basename
	{
		get
		{
			return _scale[_basenumber].Basename;
		}

		set
		{
			_basenumber = _scale.FindIndex(item => item.Basename == value);
		}
	}

	/// <summary>
	/// Oktáva noty, 0 znamená malou oktávu.
	/// </summary>
	public int Octave;

	/// <summary>
	/// Posuvka noty, záporná čísla v absolutní hodnotě značí počet béček (např. -2 znamená dvojbéčko),
	/// kladná čísla značí počet křížků (např. 1 znamená křížek), nula znamená, že nota nemá posuvku.
	/// </summary>
	public int Accidental;

	/// <summary>
	/// Poskytuje úplný název noty.
	/// </summary>
	public string Name
	{
		get
		{
			string result = Basename;  // název začíná svým kořenem
			int times = 0;
			string acc = null;  // přípona vyjadřující posuvku noty

			if (Accidental < 0) {
				times = -Accidental;
				acc = "es";  // obvyklá přípona pro každé béčko noty
				if (Basename == "e") {
					// nota e s béčkem, resp. dvojbéčkem ad., se nejmenuje "ees", nýbrž es, resp. eses atd.
					result = "es";
					--times;
				}
				else if (Basename == "a") {
					// nota a s béčkem, resp. dvojbéčkem ad., se nejmenuje "aes", nýbrž as, resp. asas atd.
					result = "as";
					acc = "as";
					--times;
				}
				else if (Basename == "h") {
					// nota h s jedním béčkem se jmenuje "b"
					if (Accidental == -1) {
						result = "b";
						--times;
					}
				}
			}
			else if (Accidental > 0) {
				// ošetření křížků (pro ně nejsou v českém názvosloví výjimky jako u béček)
				times = Accidental;
				acc = "is";
			}
			for (int i = 0; i < times; ++i) {
				// kolik posuvek nota má, tolikrát přidá správnou příponu
				result += acc;
			}
			return result;
		}
	}

	override public string ToString()
	{
		return String.Format("{0}{1}", Name, Octave);
	}

	public Note(string basename, int octave, int accidental)
	{
		Basename = basename;
		Octave = octave;
		Accidental = accidental;
	}

	/// <summary>
	/// Vrací následující notu v zadané vzdálenosti.
	/// </summary>
	/// <param name="notesNumber">Počet not od této k návratové notě.</param>
	/// <param name="halftonesNumber"> Počet půltónů od této k návratové notě.</param>
	/// <returns>Následující nota.</returns>
	public Note Shift(int notesNumber, int halftonesNumber) 
	{
		Note result = (Note) Clone();
		int sign = Math.Sign(notesNumber);
		for (int step = 0; step != notesNumber; step += sign) {
			int basenumber = result._basenumber;
			result._basenumber += sign;
			if (result._basenumber == -1) {
				result._basenumber = _scale.Count - 1;
				--result.Octave;
			}
			else if (result._basenumber == _scale.Count) {
				result._basenumber = 0;
				++result.Octave;
			}
			result.Accidental -= sign * _scale[(sign > 0) ? basenumber : result._basenumber].HalftonesNumber;
		}
		result.Accidental += halftonesNumber;
		return result;
	}

	public object Clone()
	{
		return MemberwiseClone();
	}
}
