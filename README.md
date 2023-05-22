# gemini-client
I am presenting my thought process and things I was considering when writing this project. So it is clear why I ended up implementing solution this way.
Application will display best bid and ask price after it reconstructed entire record book - I believe that displaying a price during creation of order book is a mistake, because no one is able to trade using that price.

# Order book problem
Typically order book model requires optimizing two things:
1. data structure to store different prices
2. data structure to store orders with this same price

At the beginning I was tempted to optimize 2, but I quickly realized that to be able to find the best bid and ask prices I don't really have to keep track of unique orders, I am just interested in a quantity per price. Even if I wanted to work on unique orders I don't have a unique ID of the order - which makes it hard to cancel an order w/o having a unique ID. 

# Typical models used in order book
1. Array - it guarantees insert/remove/update of order book in O(1) but finding best price is often a O(fixedsize) operation. It would recommend choosing a fixedsize of array to be able to access given price in O(1) time, and perhaps zero the array to the closing price of a previous day (if market doesn't close just choose a good price that represents current market feelings). It means that solution should be recalibrated on daily basis to be performent. It also means that there needs to be a fail over in case someone is placing an order on a price that is outside of array.

For example. If market closed with price 3600 last day, and we believe that price will not change more than 200 points during a day (one or other direction), then the array may have size 400 and we treet the price

2. Linked list
3. B-Tree or Red Black tree
4. More complex approach 


# Optimizing a order book model
When thinking about choosing a good model I had following things in mind:
1. Which prices are most important for me
2. How is exchange behaving
3. What may be needed in the future when various autotrading algorithms are introduced

1. I am interested in best bid and ask price. I imagine that I am most interested in top few best prices and not everything else. It means that for large sets of data I may consider introducing a separate small data structure (fast cache) for best price, and make sure that it is up to date fast, and not focus on everything else. 

2. Based on my test data set the best price changes on average every 3 times the price in order book is updated. It means to me that optimization for insert/remove/update of data in order book is more important then focusing on being able to get best price in minimal time. I've noticed that there are more bids then asks in order book.

Considering all of these options I believe that Tree structure gives a good balance between accessing best price and being able to update order book quickly. I assume that maybe Array is a better solution if the predefined size is well optimized.