using NotesMicroService.DTOs;
using NotesMicroService.Models;

namespace NotesMicroService.Services
{
    public class DiabetesRiskCalculator
    {
        private static readonly string[] TriggerTerms =
        {
            "Hémoglobine A1C", "Microalbumine", "Taille", "Poids",
            "Fumeur", "Fumeuse", "Anormal", "Cholestérol",
            "Vertiges", "Rechute", "Réaction", "Anticorps"
        };

        public string CalculateRisk(PatientDTO patient, IEnumerable<Note> notes)
        {
            int triggerCount = CountTriggers(notes);
            if (triggerCount == 0) return "None";
            bool isOver30 = patient.Age > 30;
            bool isMale = patient.Gender == Gender.Male;
            if (isOver30)
            {
                if (triggerCount >= 8) return "Early onset";
                if (triggerCount >= 6) return "In Danger";
                if (triggerCount >= 2) return "Borderline";
            }
            else // Under 30
            {
                if (isMale)
                {
                    if (triggerCount >= 5) return "Early onset";
                    if (triggerCount >= 3) return "In Danger";
                }
                else // Female
                {
                    if (triggerCount >= 7) return "Early onset";
                    if (triggerCount >= 4) return "In Danger";
                }
            }
            return "None";
        }

        private int CountTriggers(IEnumerable<Note> notes)
        {
            return notes
                .SelectMany(note => TriggerTerms.Where(term =>
                    note.Content.Contains(term, StringComparison.OrdinalIgnoreCase)))
                .Distinct()
                .Count();        }
    }
}