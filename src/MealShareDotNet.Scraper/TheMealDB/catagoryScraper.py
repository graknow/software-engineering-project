#Usage: catagoryScraper.py catagorty
import sys
from scraper import *
from scraper import recipe

category = sys.argv[1]

site = "https://www.themealdb.com/api/json/v1/1/filter.php?c=" + category
print(multiScrape(site))
