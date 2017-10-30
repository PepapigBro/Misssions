using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MissionsService.Models
{
    public class Mission
    {
        public int Id { get; set; }
        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        [StringLength(300)]
        public string Description { get; set; }

        public DateTimeOffset? Deadline { get; set; }

        public DateTimeOffset? DateOfCompletion { get; set; }
        
        public State TaskState { get; set; }
        public bool? isDeferFromInitial { get; set; }

        //Почему-то в экземлпдярах этот метод не отображается
       // public State ChangeState(State neededState)
       // {
       //     if (neededState == State.Canceled && TaskState==State.Waiting) { TaskState = State.Canceled; }
       //     if (neededState == State.Canceled && TaskState == State.Finished) { TaskState = State.Waiting; }
       //     if (neededState == State.Finished ) { TaskState = State.Finished; }
       //
       //     return TaskState;
       // }

    }
}