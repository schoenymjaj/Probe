using System;
using System.Linq;
using Probe.Models;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Collections.Generic;

namespace Probe.DAL
{
    public class InitializeProbeDatabaseWithSeedData : DropCreateDatabaseIfModelChanges<ProbeDataContext>
    //public class InitializeProbeDatabaseWithSeedData : DropCreateDatabaseAlways<ProbeDataContext>
    {

        protected override void Seed(ProbeDataContext context)
        {

            #region Seed the Questions and Answers
            context.QuestionType.Add(new QuestionType
            {
                Name = "MultiChoice",
                Description = "Multiple Choice",
                Questions = new List<Question>
                {
                    new ChoiceQuestion 
                    {
                        Name = "Team Allegiance",
                        Text = "What is your favorite NFL team",
                        OneChoice = true,
                        AspNetUsersId = "f61058a8-c3d1-4586-b560-a7b651f430f3",
                        Choices = new List<Choice> 
                        {
                            new Choice 
                            {
                                Name = "Bills",
                                Text = "Buffalo Bills",
                                OrderNbr = 1,
                            },
                            new Choice 
                            {
                                Name = "Eagles",
                                Text = "Philadelphia Eagles",
                                OrderNbr = 3
                            },
                            new Choice 
                            {
                                Name = "Redskins",
                                Text = "Washington Redskins",
                                OrderNbr = 2
                            },
                            new Choice 
                            {
                                Name = "Ravens",
                                Text = "Baltimore Ravens",
                                OrderNbr = 4
                            }
                        }
                    },
                    new ChoiceQuestion 
                    {
                        Name = "Political Party",
                        Text = "What political party are you associated with",
                        OneChoice = true,
                        AspNetUsersId = "f61058a8-c3d1-4586-b560-a7b651f430f3",
                        Choices = new List<Choice> 
                        {
                            new Choice 
                            {
                                Name = "GOP",
                                Text = "Replican Party",
                                OrderNbr = 2,
                            },
                            new Choice 
                            {
                                Name = "Democratic",
                                Text = "Democratic Party",
                                OrderNbr = 1,
                            }
                        }
                    },
                    new ChoiceQuestion 
                    {
                        Name = "Closure Issue",
                        Text = "Do you put on both socks before you put on your shoes",
                        OneChoice = true,
                        AspNetUsersId = "f61058a8-c3d1-4586-b560-a7b651f430f3",
                        Choices = new List<Choice> 
                        {
                            new Choice 
                            {
                                Name = "Yes",
                                Text = "Yes",
                                OrderNbr = 1,
                            },
                            new Choice 
                            {
                                Name = "No",
                                Text = "No",
                                OrderNbr = 2,
                            }
                        }

                    },
                    new ChoiceQuestion 
                    {
                        Name = "Favorite Color",
                        Text = "What's your favorite color",
                        OneChoice = true,
                        AspNetUsersId = "f61058a8-c3d1-4586-b560-a7b651f430f3",
                        Choices = new List<Choice> 
                        {
                            new Choice 
                            {
                                Name = "Red",
                                Text = "Red",
                                OrderNbr = 2,
                            },
                            new Choice 
                            {
                                Name = "Blue",
                                Text = "Blue",
                                OrderNbr = 1,
                            },
                            new Choice 
                            {
                                Name = "Green",
                                Text = "Green",
                                OrderNbr = 3,
                            },
                             new Choice 
                            {
                                Name = "Yellow",
                                Text = "Yellow",
                                OrderNbr = 4,
                            },
                            new Choice 
                            {
                                Name = "Orange",
                                Text = "Orange",
                                OrderNbr = 5,
                            },
                            new Choice 
                            {
                                Name = "Purple",
                                Text = "Purple",
                                OrderNbr = 6,
                            },
                            new Choice 
                            {
                                Name = "Gold",
                                Text = "Gold",
                                OrderNbr = 7,
                            },
                            new Choice 
                            {
                                Name = "Brown",
                                Text = "Brown",
                                OrderNbr = 8,
                            }
                       }

                    },
                    new ChoiceQuestion 
                    {
                        Name = "Driving Speed",
                        Text = "How fast do you drive your car",
                        OneChoice = true,
                        AspNetUsersId = "f61058a8-c3d1-4586-b560-a7b651f430f3",
                        Choices = new List<Choice>
                        {
                            new Choice 
                            {
                                Name = "Slow Speed",
                                Text = "20 mph",
                                OrderNbr = 1,
                            },
                            new Choice 
                            {
                                Name = "Medium Speed",
                                Text = "40 mph",
                                OrderNbr = 2,
                            },
                            new Choice 
                            {
                                Name = "Fast Speed",
                                Text = "60 mph",
                                OrderNbr = 3,
                            }
                        }
                    },
                    new ChoiceQuestion 
                    {
                        Name = "Risk Taker",
                        Text = "How much of a risk taker are you",
                        OneChoice = true,
                        AspNetUsersId = "f61058a8-c3d1-4586-b560-a7b651f430f3",
                        Choices = new List<Choice>
                        {
                            new Choice 
                            {
                                Name = "Conservative",
                                Text = "Conservative",
                                OrderNbr = 2,
                            },
                            new Choice 
                            {
                                Name = "No Risk",
                                Text = "No Risk",
                                OrderNbr = 1,
                            },
                            new Choice 
                            {
                                Name = "High  Risk",
                                Text = "Live by the seat of my pants",
                                OrderNbr = 3,
                            }
                        }
                    },
                    new ChoiceQuestion 
                    {
                        Name = "Spiritual Measure",
                        Text = "Where do you sit in your choice of religious services",
                        OneChoice = true,
                        AspNetUsersId = "f61058a8-c3d1-4586-b560-a7b651f430f3",
                        Choices = new List<Choice>
                        {
                            new Choice 
                            {
                                Name = "Front",
                                Text = "Front Row",
                                OrderNbr = 1,
                            },
                            new Choice 
                            {
                                Name = "Middle",
                                Text = "Middle",
                                OrderNbr = 2,
                            },
                            new Choice 
                            {
                                Name = "Back",
                                Text = "Back",
                                OrderNbr = 3,
                            }
                        }
                    },
                    new ChoiceQuestion 
                    {
                        Name = "Number of Kids",
                        Text = "How many kids would you like to raise",
                        OneChoice = true,
                        AspNetUsersId = "f61058a8-c3d1-4586-b560-a7b651f430f3",
                        Choices = new List<Choice>
                        {
                            new Choice 
                            {
                                Name = "None",
                                Text = "None",
                                OrderNbr = 1,
                            },
                            new Choice 
                            {
                                Name = "One",
                                Text = "One",
                                OrderNbr = 2,
                            },
                            new Choice 
                            {
                                Name = "Two",
                                Text = "Two",
                                OrderNbr = 3,
                            },
                            new Choice 
                            {
                                Name = "Four",
                                Text = "Four",
                                OrderNbr = 4,
                            },
                            new Choice 
                            {
                                Name = "Lots",
                                Text = "As many as can fit in our house",
                                OrderNbr = 5,
                            }
                        }
                    },
                    new ChoiceQuestion 
                    {
                        Name = "Favorite Food",
                        Text = "What is your favorite type of food",
                        OneChoice = true,
                        AspNetUsersId = "f61058a8-c3d1-4586-b560-a7b651f430f3",
                        Choices = new List<Choice>
                        {
                            new Choice 
                            {
                                Name = "American",
                                Text = "American",
                                OrderNbr = 1,
                            },
                            new Choice 
                            {
                                Name = "Chinese",
                                Text = "Chinese",
                                OrderNbr = 2,
                            },
                            new Choice 
                            {
                                Name = "Italian",
                                Text = "Italian",
                                OrderNbr = 3,
                            },
                            new Choice 
                            {
                                Name = "Mexican",
                                Text = "Mexican",
                                OrderNbr = 4,
                            },
                            new Choice 
                            {
                                Name = "Thai",
                                Text = "Thai",
                                OrderNbr = 5,
                            }
                        }
                    },//new choice
                    new ChoiceQuestion 
                    {
                        Name = "Weather",
                        Text = "What weather do you feel most comfortable",
                        OneChoice = true,
                        AspNetUsersId = "f61058a8-c3d1-4586-b560-a7b651f430f3",
                        Choices = new List<Choice>
                        {
                            new Choice 
                            {
                                Name = "Equator",
                                Text = "Equator-like Heat",
                                OrderNbr = 1,
                            },
                            new Choice 
                            {
                                Name = "Hot and Humid",
                                Text = "Hot and Humid",
                                OrderNbr = 2,
                            },
                            new Choice 
                            {
                                Name = "Hot and Dry",
                                Text = "Hot and Dry",
                                OrderNbr = 3,
                            },
                            new Choice 
                            {
                                Name = "Seasonal",
                                Text = "Full Seasons",
                                OrderNbr = 4,
                            },
                            new Choice 
                            {
                                Name = "Mild",
                                Text = "Mild",
                                OrderNbr = 5,
                            },
                            new Choice 
                            {
                                Name = "Cold",
                                Text = "Cold",
                                OrderNbr = 6,
                            },
                            new Choice 
                            {
                                Name = "Antartica",
                                Text = "Antartica-like Cold",
                                OrderNbr = 7,
                            }
                        }
                    },//new choice
                    new ChoiceQuestion 
                    {
                        Name = "Water Expert",
                        Text = "Which bottle is the bottled designer water",
                        OneChoice = true,
                        AspNetUsersId = "f61058a8-c3d1-4586-b560-a7b651f430f3",
                        Choices = new List<Choice>
                        {
                            new Choice 
                            {
                                Name = "Water Cup #1",
                                Text = "Water Cup #1 (Correct)",
                                OrderNbr = 1,
                                Correct = true
                            },
                            new Choice 
                            {
                                Name = "Water Cup #2",
                                Text = "Water Cup #2",
                                OrderNbr = 2,
                            }
                        }
                    },//new choice
                    new ChoiceQuestion 
                    {
                        Name = "Wine Expert",
                        Text = "Which glass is the Chardoney Premier Crue",
                        OneChoice = true,
                        AspNetUsersId = "f61058a8-c3d1-4586-b560-a7b651f430f3",
                        Choices = new List<Choice>
                        {
                            new Choice 
                            {
                                Name = "Wine Cup #1",
                                Text = "Wine Cup #1",
                                OrderNbr = 1,
                            },
                            new Choice 
                            {
                                Name = "Wine Cup #2",
                                Text = "Wine Cup #2 (Correct)",
                                OrderNbr = 2,
                                Correct = true
                            }
                        }
                    }//new choice
                }
            });

            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException e) 
            {
                foreach (var error in e.EntityValidationErrors)
                {
                    System.Diagnostics.Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        error.Entry.Entity.GetType().Name, error.Entry.State);

                    foreach (var ve in error.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("-Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage); 
                    }

                }
            }

            #endregion

            #region Seed the Game Types
            //context.GameType.Add(new GameType
            //{
            //    Name = "Match",
            //    Description = "Players answer a series of questions. Compatible matches are found amongst players.",
            //    Games = new List<Game> 
            //        {
            //           new Game 
            //           {
            //               Name = "You're Full of Crap Party - Match",
            //               Description = "At 9510 Red Apple Lane. It's a tasting contest",
            //               AspNetUsersId = "f61058a8-c3d1-4586-b560-a7b651f430f3",
            //               GameConfigurations = new List<GameConfiguration>
            //               {
            //                   new GameConfiguration
            //                   {
            //                       Name = "DeviceCanPlayGameOnlyOnce",
            //                       Value = "false"
            //                   }
            //               }
            //           },
            //           new Game 
            //           {
            //               Name = "You're Full of Crap Party II - Match",
            //               Description = "At 9510 Red Apple Lane. It's a tasting contest",
            //               AspNetUsersId = "f61058a8-c3d1-4586-b560-a7b651f430f3",
            //               GameConfigurations = new List<GameConfiguration>
            //               {
            //                   new GameConfiguration
            //                   {
            //                       Name = "DeviceCanPlayGameOnlyOnce",
            //                       Value = "false"
            //                   }
            //               }
            //           }
            //        }
            //});
            //context.GameType.Add(new GameType
            //{
            //    Name = "Test",
            //    Description = "Players answer a series of questions. Assess the players competance of the topic(s) based on percentage correct answers.",
            //    Games = new List<Game> 
            //        {
            //           new Game 
            //           {
            //               Name = "You're Full of Crap Party - Test",
            //               Description = "At 9510 Red Apple Lane. It's a tasting contest",
            //               AspNetUsersId = "f61058a8-c3d1-4586-b560-a7b651f430f3",
            //               GameConfigurations = new List<GameConfiguration>
            //               {
            //                   new GameConfiguration
            //                   {
            //                       Name = "DeviceCanPlayGameOnlyOnce",
            //                       Value = "false"
            //                   }
            //               }
            //           }
            //        }
            //});



            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var error in e.EntityValidationErrors)
                {
                    System.Diagnostics.Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        error.Entry.Entity.GetType().Name, error.Entry.State);

                    foreach (var ve in error.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("-Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }

                }
            }

            #endregion

            #region Seed the Questions for the Games

            /*You're Full of Crap Party - Match*/
            context.GameQuestion.Add(new GameQuestion
            {
                GameId = context.Game.Where(g => g.Name == "You're Full of Crap Party - Match").Select(g => g.Id).Single(),
                QuestionId = context.Question.Where(q => q.Name == "Closure Issue").Select(g => g.Id).Single(),
                OrderNbr = 3,
                Weight = 10
            });

            context.GameQuestion.Add(new GameQuestion
            {
                GameId = context.Game.Where(g => g.Name == "You're Full of Crap Party - Match").Select(g => g.Id).Single(),
                QuestionId = context.Question.Where(q => q.Name == "Number of Kids").Select(g => g.Id).Single(),
                OrderNbr = 1,
                Weight = 10
            });

            context.GameQuestion.Add(new GameQuestion
            {
                GameId = context.Game.Where(g => g.Name == "You're Full of Crap Party - Match").Select(g => g.Id).Single(),
                QuestionId = context.Question.Where(q => q.Name == "Risk Taker").Select(g => g.Id).Single(),
                OrderNbr = 2,
                Weight = 10
            });

            context.GameQuestion.Add(new GameQuestion
            {
                GameId = context.Game.Where(g => g.Name == "You're Full of Crap Party - Match").Select(g => g.Id).Single(),
                QuestionId = context.Question.Where(q => q.Name == "Team Allegiance").Select(g => g.Id).Single(),
                OrderNbr = 4,
                Weight = 10
            });

            /*You're Full of Crap Party II - Match*/
            context.GameQuestion.Add(new GameQuestion
            {
                GameId = context.Game.Where(g => g.Name == "You're Full of Crap Party II - Match").Select(g => g.Id).Single(),
                QuestionId = context.Question.Where(q => q.Name == "Closure Issue").Select(g => g.Id).Single(),
                OrderNbr = 3,
                Weight = 10
            });

            context.GameQuestion.Add(new GameQuestion
            {
                GameId = context.Game.Where(g => g.Name == "You're Full of Crap Party II - Match").Select(g => g.Id).Single(),
                QuestionId = context.Question.Where(q => q.Name == "Number of Kids").Select(g => g.Id).Single(),
                OrderNbr = 1,
                Weight = 10
            });

            context.GameQuestion.Add(new GameQuestion
            {
                GameId = context.Game.Where(g => g.Name == "You're Full of Crap Party II - Match").Select(g => g.Id).Single(),
                QuestionId = context.Question.Where(q => q.Name == "Risk Taker").Select(g => g.Id).Single(),
                OrderNbr = 2,
                Weight = 10
            });

            context.GameQuestion.Add(new GameQuestion
            {
                GameId = context.Game.Where(g => g.Name == "You're Full of Crap Party II - Match").Select(g => g.Id).Single(),
                QuestionId = context.Question.Where(q => q.Name == "Team Allegiance").Select(g => g.Id).Single(),
                OrderNbr = 4,
                Weight = 10
            });

            context.GameQuestion.Add(new GameQuestion
            {
                GameId = context.Game.Where(g => g.Name == "You're Full of Crap Party II - Match").Select(g => g.Id).Single(),
                QuestionId = context.Question.Where(q => q.Name == "Weather").Select(g => g.Id).Single(),
                OrderNbr = 5,
                Weight = 10
            });

            context.GameQuestion.Add(new GameQuestion
            {
                GameId = context.Game.Where(g => g.Name == "You're Full of Crap Party II - Match").Select(g => g.Id).Single(),
                QuestionId = context.Question.Where(q => q.Name == "Favorite Food").Select(g => g.Id).Single(),
                OrderNbr = 6,
                Weight = 10
            });

            context.GameQuestion.Add(new GameQuestion
            {
                GameId = context.Game.Where(g => g.Name == "You're Full of Crap Party II - Match").Select(g => g.Id).Single(),
                QuestionId = context.Question.Where(q => q.Name == "Driving Speed").Select(g => g.Id).Single(),
                OrderNbr = 7,
                Weight = 10
            });

            context.GameQuestion.Add(new GameQuestion
            {
                GameId = context.Game.Where(g => g.Name == "You're Full of Crap Party II - Match").Select(g => g.Id).Single(),
                QuestionId = context.Question.Where(q => q.Name == "Spiritual Measure").Select(g => g.Id).Single(),
                OrderNbr = 8,
                Weight = 10
            });


            /*You're Full of Crap Party - Test*/
            context.GameQuestion.Add(new GameQuestion
            {
                GameId = context.Game.Where(g => g.Name == "You're Full of Crap Party - Test").Select(g => g.Id).Single(),
                QuestionId = context.Question.Where(q => q.Name == "Water Expert").Select(g => g.Id).Single(),
                OrderNbr = 1,
                Weight = 10
            });
            context.GameQuestion.Add(new GameQuestion
            {
                GameId = context.Game.Where(g => g.Name == "You're Full of Crap Party - Test").Select(g => g.Id).Single(),
                QuestionId = context.Question.Where(q => q.Name == "Wine Expert").Select(g => g.Id).Single(),
                OrderNbr = 2,
                Weight = 10
            });


            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var error in e.EntityValidationErrors)
                {
                    System.Diagnostics.Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        error.Entry.Entity.GetType().Name, error.Entry.State);

                    foreach (var ve in error.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("-Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }

                }
            }

            #endregion

            #region Seed Game Plays and Players for the Game "You're Full of Crap Party - Match"

            //Seed for the game "You're Full of Crap Party"
            long gameId = context.Game.Where(g => g.Name == "You're Full of Crap Party - Match").Select(g => g.Id).Single();
            long qTeamAllegiance = context.Question.SingleOrDefault(q => q.Name == "Team Allegiance").Id;
            long choiceRedskins = context.Choice.SingleOrDefault(c => c.ChoiceQuestionId == qTeamAllegiance && c.Name == "Redskins").Id;

            context.ReportType.Add(new ReportType
            {
                Name = "Match Report 1",
                Description = "Graphic report that identifies each player and the most and least compatible matches.",
                Games = new List<Game> 
                    {
                       new Game 
                       {
                           Name = "#1 - You're Full of Crap Party - Match",
                           Description = "One of many Full of Crap Party Matches",
                           Code = "11111",

                           StartDate = Convert.ToDateTime("7/13/2013 8:00PM"),
                           EndDate = Convert.ToDateTime("7/13/2015 8:00PM"),

                           //Players = new List<Player>
                           //{
                           //    new Player 
                           //    {
                           //        LastName = "Mitchell",
                           //        FirstName = "Bobby",
                           //        NickName = "BMitch",
                           //        EmailAddr = "bmitchell@gmail.com",
                           //        SubmitDate = DateTime.Now.Date,
                           //        SubmitTime = DateTime.Parse("9:00 PM"),
                           //        GamePlayAnswers = new List<GamePlayAnswer> 
                           //        {
                           //            new GamePlayAnswer 
                           //            {
                           //                ChoiceId = choiceRedskins
                           //            }
                           //        }
                           //    },
                           //    new Player 
                           //    {
                           //        LastName = "Mitchell",
                           //        FirstName = "Caren",
                           //        NickName = "AskCaren",
                           //        EmailAddr = "cmitchell@gmail.com",
                           //        SubmitDate = DateTime.Now.Date,
                           //        SubmitTime = DateTime.Parse("9:00 PM")
                           //    },
                           //    new Player 
                           //    {
                           //        LastName = "McClanahan",
                           //        FirstName = "Tim",
                           //        NickName = "Timmy",
                           //        EmailAddr = "tmcclanahan@gmail.com",
                           //        SubmitDate = DateTime.Now.Date,
                           //        SubmitTime = DateTime.Parse("9:00 PM")
                           //    },                               
                           //    new Player 
                           //    {
                           //        LastName = "Kaufman",
                           //        FirstName = "Steve",
                           //        NickName = "Stever",
                           //        EmailAddr = "skaufman@gmail.com",
                           //        SubmitDate = DateTime.Now.Date,
                           //        SubmitTime = DateTime.Parse("9:00 PM")
                           //    },
                           //}
                       },
                       new Game 
                       {
                           Name = "#2 - You're Full of Crap Party - Match",
                           Description = "Two of many Full of Crap Party Matches",
                           Code = "22222",

                           StartDate = Convert.ToDateTime("7/13/2013 8:00PM"),
                           EndDate = Convert.ToDateTime("7/13/2015 8:00PM"),

                       },
                       new Game
                       {
                           Name = "#3 - You're Full of Crap Party - Match",
                           Description = "Three of many Full of Crap Parties",
                           Code = "33333",

                           StartDate = Convert.ToDateTime("7/13/2013 8:00PM"),
                           EndDate = Convert.ToDateTime("7/13/2015 8:00PM"),

                       },
                       //new GamePlay 
                       //{
                       //    Name = "#4 - You're Full of Crap Party - Match",
                       //    Description = "Four of many Full of Crap Party Matches",
                       //    Code = "44444",

                       //    StartDate = Convert.ToDateTime("7/13/2013 8:00PM"),
                       //    EndDate = Convert.ToDateTime("7/13/2015 8:00PM"),

                       //    GameId = gameId,
                       //},
                       //new GamePlay 
                       //{
                       //    Name = "#5 - You're Full of Crap Party - Match",
                       //    Description = "Five of many Full of Crap Party Matches",
                       //    Code = "55555",

                       //    StartDate = Convert.ToDateTime("7/13/2013 8:00PM"),
                       //    EndDate = Convert.ToDateTime("7/13/2015 8:00PM"),

                       //    GameId = gameId,
                       //},
                       //new GamePlay 
                       //{
                       //    Name = "#6 - You're Full of Crap Party - Match",
                       //    Description = "Six of many Full of Crap Party Matches",
                       //    Code = "66666",

                       //    StartDate = Convert.ToDateTime("7/13/2013 8:00PM"),
                       //    EndDate = Convert.ToDateTime("7/13/2015 8:00PM"),

                       //    GameId = gameId,
                       //}
                    }
            });//context.ReportType.Add(new ReportType

            #endregion

            #region Seed Game Plays and Players for the Game "You're Full of Crap Party - Test"

            //Seed for the game "You're Full of Crap Party"
            long gameTestId = context.Game.Where(g => g.Name == "You're Full of Crap Party - Test").Select(g => g.Id).Single();

            context.ReportType.Add(new ReportType
            {
                Name = "Test Report 1",
                Description = "Graphic report that identifies each player and their test scores.",
                Games = new List<Game> 
                    {
                       new Game 
                       {
                           Name = "#1 - You're Full of Crap Party - Test",
                           Description = "One of many Full of Crap Party Tests",
                           Code = "AAAAA",

                           StartDate = Convert.ToDateTime("7/13/2013 8:00PM"),
                           EndDate = Convert.ToDateTime("7/13/2015 8:00PM"),

                       },
                       new Game 
                       {
                           Name = "#2 - You're Full of Crap Party - Test",
                           Description = "Two of many Full of Crap Party Tests",
                           Code = "BBBBB",

                           StartDate = Convert.ToDateTime("7/13/2013 8:00PM"),
                           EndDate = Convert.ToDateTime("7/13/2015 8:00PM"),

                       },
                       //new GamePlay 
                       //{
                       //    Name = "#3 - You're Full of Crap Party - Test",
                       //    Description = "Three of many Full of Crap Party Tests",
                       //    Code = "CCCCC",

                       //    StartDate = Convert.ToDateTime("7/13/2013 8:00PM"),
                       //    EndDate = Convert.ToDateTime("7/13/2015 8:00PM"),

                       //    GameId = gameTestId,
                       //},
                       //new GamePlay 
                       //{
                       //    Name = "#4 - You're Full of Crap Party - Test",
                       //    Description = "Four of many Full of Crap Party Tests",
                       //    Code = "DDDDD",

                       //    StartDate = Convert.ToDateTime("7/13/2013 8:00PM"),
                       //    EndDate = Convert.ToDateTime("7/13/2015 8:00PM"),

                       //    GameId = gameTestId,
                       //},
                       //new GamePlay 
                       //{
                       //    Name = "#5 - You're Full of Crap Party - Test",
                       //    Description = "Five of many Full of Crap Party Tests",
                       //    Code = "EEEEE",

                       //    StartDate = Convert.ToDateTime("7/13/2013 8:00PM"),
                       //    EndDate = Convert.ToDateTime("7/13/2015 8:00PM"),

                       //    GameId = gameTestId,
                       //},
                       //new GamePlay 
                       //{
                       //    Name = "#6 - You're Full of Crap Party - Test",
                       //    Description = "Six of many Full of Crap Party Tests",
                       //    Code = "FFFFF",

                       //    StartDate = Convert.ToDateTime("7/13/2013 8:00PM"),
                       //    EndDate = Convert.ToDateTime("7/13/2015 8:00PM"),

                       //    GameId = gameTestId,
                       //},
                       //new GamePlay 
                       //{
                       //    Name = "#7 - You're Full of Crap Party - Test",
                       //    Description = "Seven of many Full of Crap Party Tests",
                       //    Code = "GGGGG",

                       //    StartDate = Convert.ToDateTime("7/13/2013 8:00PM"),
                       //    EndDate = Convert.ToDateTime("7/13/2015 8:00PM"),

                       //    GameId = gameTestId,
                       //},
                       //new GamePlay 
                       //{
                       //    Name = "#8 - You're Full of Crap Party - Test",
                       //    Description = "Eight of many Full of Crap Party Tests",
                       //    Code = "HHHHH",

                       //    StartDate = Convert.ToDateTime("7/13/2013 8:00PM"),
                       //    EndDate = Convert.ToDateTime("7/13/2015 8:00PM"),

                       //    GameId = gameTestId,
                       //},
                       //new GamePlay 
                       //{
                       //    Name = "#9 - You're Full of Crap Party - Test",
                       //    Description = "Nine of many Full of Crap Party Tests",
                       //    Code = "IIIII",

                       //    StartDate = Convert.ToDateTime("7/13/2013 8:00PM"),
                       //    EndDate = Convert.ToDateTime("7/13/2015 8:00PM"),

                       //    GameId = gameTestId,
                       //},
                       //new GamePlay 
                       //{
                       //    Name = "#10 - You're Full of Crap Party - Test",
                       //    Description = "Ten of many Full of Crap Party Tests",
                       //    Code = "JJJJJ",

                       //    StartDate = Convert.ToDateTime("7/13/2013 8:00PM"),
                       //    EndDate = Convert.ToDateTime("7/13/2015 8:00PM"),

                       //    GameId = gameTestId,
                       //},
                       //new GamePlay 
                       //{
                       //    Name = "#11 - You're Full of Crap Party - Test",
                       //    Description = "Eleven of many Full of Crap Party Tests",
                       //    Code = "KKKKK",

                       //    StartDate = Convert.ToDateTime("7/13/2013 8:00PM"),
                       //    EndDate = Convert.ToDateTime("7/13/2015 8:00PM"),

                       //    GameId = gameTestId,
                       //},
                       //new GamePlay 
                       //{
                       //    Name = "#12 - You're Full of Crap Party - Test",
                       //    Description = "Twelve of many Full of Crap Party Tests",
                       //    Code = "LLLLL",

                       //    StartDate = Convert.ToDateTime("7/13/2013 8:00PM"),
                       //    EndDate = Convert.ToDateTime("7/13/2015 8:00PM"),

                       //    GameId = gameTestId,
                       //}
                    }
            });//context.ReportType.Add(new ReportType

            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var error in e.EntityValidationErrors)
                {
                    System.Diagnostics.Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        error.Entry.Entity.GetType().Name, error.Entry.State);

                    foreach (var ve in error.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("-Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }

                }
            }

            #endregion

        }
    }
}