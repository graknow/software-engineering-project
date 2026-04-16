#Usage: mealIdScraper.py mealID
import sys
from scraper import *
from scraper import recipe
import argparse

parser = argparse.ArgumentParser(
                    prog='MealID Scraper',
                    description='Grabs the recipe from TheMealDB based on the mealID of the recipe',
                    epilog='Stop reading this, it is a waste of your time')

parser.add_argument("MealID", help="The MealID we are searching for")
args = parser.parse_args()

mealID = args.MealID

site = "https://www.themealdb.com/api/json/v1/1/lookup.php?i=" + mealID
print(scrape(site))
