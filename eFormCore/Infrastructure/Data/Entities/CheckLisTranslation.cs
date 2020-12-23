using System.ComponentModel.DataAnnotations.Schema;

namespace Microting.eForm.Infrastructure.Data.Entities
{
    public class CheckLisTranslation : PnBase
    {
        [ForeignKey("CheckList")]
        public int CheckListId { get; set; }

        [ForeignKey("Language")]
        public int LanguageId { get; set; }

        public string Text { get; set; }

        public virtual CheckList CheckList { get; set; }
    }
}