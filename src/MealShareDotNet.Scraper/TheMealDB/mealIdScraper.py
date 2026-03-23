#Usage: mealIdScraper.py mealID
import sys
from scraper import *
from scraper import recipe

mealID = sys.argv[1]

site = "https://www.themealdb.com/api/json/v1/1/lookup.php?i=" + mealID
print(scrape(site))
