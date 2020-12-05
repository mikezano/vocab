   
# ![](wwwroot/images/favicon.png)ocab  

A .NET 5 Blazor WebAssembly project that allows you to create a set of multiple choice flashcards based off of a configured google sheet.

# Demo

[zano.azurewebsites.net](https://zano.azurewebsites.net)

<img src="wwwroot/images/demo.gif" width=400/>

## Instructions

1. Go to your google sheet and copy the equivalent highlighted portion of the url. This is your **google sheet id**. Use the first two columns to build a mapping for your card deck. 🤓

   ![](wwwroot/images/sheet-id.png)

2. Publish your google sheet through (File --> Publish to Web)

3. Enter the **google sheet id** into the initial input of the Vocab site and hit the start button.

    ![](wwwroot/images/home.png)

### Why ?

I like playing video games with spanish audio/text and am always picking up on new words as I do.  I figured why not collect them into some app to help me remember them, hence this Vocab app.  Ideally I'd like to get these words from a google translate API that gives me access to my phrasebook but it [doesn't seem like that's possible](https://issuetracker.google.com/issues/35902966) ... yet.  For now the google sheet solution works well :)
