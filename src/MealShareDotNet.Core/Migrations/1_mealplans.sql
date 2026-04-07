CREATE TABLE MealPlans
(
    Id INTEGER PRIMARY KEY ASC AUTOINCREMENT NOT NULL,
    RecipeId INTEGER NOT NULL,
    EventName TEXT,
    ScheduledTime TEXT NOT NULL,
    FOREIGN KEY(RecipeId) REFERENCES Recipes(Id)
);

CREATE INDEX ScheduledTimeIdx ON MealPlans(ScheduledTime);
