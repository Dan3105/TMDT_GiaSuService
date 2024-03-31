using System;
using System.Collections.Generic;

namespace GiaSuService.EntityModel;

public partial class District
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Provinceid { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual Province Province { get; set; } = null!;

    public virtual ICollection<Tutorrequestform> Tutorrequestforms { get; set; } = new List<Tutorrequestform>();

    public virtual ICollection<Tutor> Tutors { get; set; } = new List<Tutor>();

    public virtual ICollection<Tutor> TutorsNavigation { get; set; } = new List<Tutor>();
}
