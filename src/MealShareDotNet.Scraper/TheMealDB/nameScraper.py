#Usage: nameScraper.py name
import sys
from scraper import *
from scraper import recipe

name = sys.argv[1]

site = "https://www.themealdb.com/api/json/v1/1/search.php?s=" + name
print(multiScrape(site))
