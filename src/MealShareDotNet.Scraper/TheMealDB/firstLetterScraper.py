#Usage: firstLetterScraper.py firstLetter
import sys
from scraper import *
from scraper import recipe

firstLetter = sys.argv[1]

site = "https://www.themealdb.com/api/json/v1/1/search.php?f=" + firstLetter
print(multiScrape(site))
