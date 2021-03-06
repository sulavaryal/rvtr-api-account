using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RVTR.Account.ObjectModel.Models
{
  /// <summary>
  /// Represents the _Profile_ model
  /// </summary>
  public class ProfileModel : IValidatableObject
  {
    [ForeignKey("NameModel")]
    public int Id { get; set; }

    public string Email { get; set; }

    public NameModel Name { get; set; }

    public string Phone { get; set; }
    public string Age { get; set; }
    public string Image { get; set; }

    public int? AccountId { get; set; }

    public virtual AccountModel Account { get; set; }

    /// <summary>
    /// Represents the _Profile_ `Validate` method
    /// </summary>
    /// <param name="validationContext"></param>
    /// <returns></returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) => new List<ValidationResult>();
  }
}
