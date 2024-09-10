namespace App_plateforme_de_recurtement.Models
{
    public class QuestionWithAnswers
    {
        public int QuestionId { get; set; }
        public string Query { get; set; }
        public List<Answer> Answers { get; set; }
        public List<Answercandidat> CandidateAnswers { get; set; }
    }

}
