/*
The MIT License (MIT)

Copyright (c) 2007 - 2019 Microting A/S

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Microting.eForm.Infrastructure.Data.Entities
{
    public partial class options : BaseEntity
    {
        public int NextQuestionId { get; set; }
        
        public int Weight { get; set; }
        
        public int WeightValue { get; set; }
        
        public int ContinuousOptionId { get; set; }
        
        [ForeignKey("question")]
        public int QuestionId { get; set; }
        
        public int OptionsIndex { get; set; }
        
        public virtual questions Question { get; set; }

        public void Create(MicrotingDbAnySql dbContext)
        {
            WorkflowState = Constants.Constants.WorkflowStates.Created;
            Version = 1;
            
            QuestionId = QuestionId;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;

            dbContext.options.Add(this);
            dbContext.SaveChanges();

            dbContext.option_versions.Add(MapVersions(this));
            dbContext.SaveChanges();
        }

        public void Update(MicrotingDbAnySql dbContext)
        {
            options option = dbContext.options.FirstOrDefault(x => x.Id == Id);

            if (option == null)
            {
                throw new NullReferenceException($"Could not find option with Id: {Id}");
            }

            option.QuestionId = QuestionId;
            option.Weight = Weight;
            option.WeightValue = WeightValue;
            option.NextQuestionId = NextQuestionId;
            option.ContinuousOptionId = ContinuousOptionId;
            option.OptionsIndex = OptionsIndex;

            if (dbContext.ChangeTracker.HasChanges())
            {
                Version += 1;
                UpdatedAt = DateTime.Now;

                dbContext.option_versions.Add(MapVersions(option));
                dbContext.SaveChanges();
            }

        }

        public void Delete(MicrotingDbAnySql dbContext)
        {
            options option = dbContext.options.FirstOrDefault(x => x.Id == Id);

            if (option == null)
            {
                throw new NullReferenceException($"Could not find option with Id: {Id}");
            }

            option.WorkflowState = Constants.Constants.WorkflowStates.Removed;
            
            if (dbContext.ChangeTracker.HasChanges())
            {
                Version += 1;
                UpdatedAt = DateTime.Now;

                dbContext.option_versions.Add(MapVersions(option));
                dbContext.SaveChanges();
            }
        }

        private option_versions MapVersions(options option)
        {
            option_versions optionVersions = new option_versions();

            optionVersions.QuestionId = option.QuestionId;
            optionVersions.Weight = option.Weight;
            optionVersions.WeightValue = option.WeightValue;
            optionVersions.NextQuestionId = option.NextQuestionId;
            optionVersions.ContinuousOptionId = option.ContinuousOptionId;
            optionVersions.OptionsIndex = option.OptionsIndex;
            optionVersions.OptionId = option.Id;

            return optionVersions;

        }
    }
}