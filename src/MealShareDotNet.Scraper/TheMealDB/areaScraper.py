#Usage: areaScraper.py area
import sys
from scraper import *
from scraper import recipe

area = sys.argv[1]

site = "https://www.themealdb.com/api/json/v1/1/filter.php?a=" + area
print(multiScrape(site))
