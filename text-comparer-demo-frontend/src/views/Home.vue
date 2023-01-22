<template>
  <div class="home">
    <div v-if="!showOutput" class="textcomparerinput">
      <textarea v-model="text1"></textarea>
      <textarea v-model="text2"></textarea>
      <button type="button" @click="onCompareButtonClicked">Compare the two specified texts</button>
    </div>
    <div v-else class="textcompareroutput">
      <button type="button" @click="onBackButtonClicked">Back</button>
      <table class="monospacedtable textcomparer">
        <tr v-for="compareResultLine in compareResult" :key="compareResultLine.key">
          <td class="linenumber"><span v-if="compareResultLine.lineOfText1">{{ compareResultLine.lineOfText1.lineNumber }}</span>
            <span v-else class="linenonexisting"></span>
          </td>
          <td class="linetext" v-if="compareResultLine.lineOfText1" :class="compareResultLine.lineOfText1.aggregatedResultType">
          <span v-if="compareResultLine.lineOfText1">
            <span v-for="range in compareResultLine.lineOfText1.lineTexts" :key="range.key" class="keep-spaces"
                  :class="{whitespaceonly: (range.text.trim().length<=0)}">
              <span v-if="range.text.length>0" :class="range.comparisonType">{{ range.text }}</span>
            </span>
          </span>
          </td>
          <td v-else class="linenonexisting"></td>
          <td class="linenumber">
            <span v-if="compareResultLine.lineOfText2">{{ compareResultLine.lineOfText2.lineNumber }}</span>
            <span v-else class="linenonexisting"></span>
          </td>
          <td class="linetext" v-if="compareResultLine.lineOfText2" :class="compareResultLine.lineOfText2.aggregatedResultType">
          <span v-if="compareResultLine.lineOfText2">
            <span v-for="range in compareResultLine.lineOfText2.lineTexts" :key="range.key" class="keep-spaces"
                  :class="{whitespaceonly: (range.text.trim().length<=0)}">
              <span v-if="range.text.length>0" :class="range.comparisonType">{{ range.text }}</span>
            </span>
          </span></td>
          <td v-else class="linenonexisting"></td>
        </tr>
      </table>
    </div>
  </div>
</template>

<style>
.monospacedtable,
.textcomparerinput textarea {
  font-family: 'Source Code Pro', monospace;
  font-size: 1em;
}

.textcomparerinput textarea {
  padding: 1em;
}

.textcomparer .linenumber {
  text-align: right;
}

.textcomparer .linetext {
  text-align: left;
  padding-left: 2px;
}

.textcomparer {
  width: 100%;
}

.textcomparer td {
  padding-left: 1em;
  padding-right: 1em;
}

.textcomparer td.Addition {
  background-color: #f4ffe5;
}

.textcomparer td.Different {
  background-color: #ffebeb;
}

.textcomparer span.Addition {
  background-color: #cafd82;
  border-color: #68b102;
  border-style: solid;
  border-width: 2px;
  border-spacing: 2px;
}

.textcomparer .whitespaceonly span.Addition {
  border-style: none;
}

.textcomparer span.Equals {
}

.textcomparer span.Different {
  background-color: #ff8080;
  border-color: red;
  border-style: solid;
  border-width: 2px;
  border-spacing: 2px;
}

.linenonexisting {
  background-color: whitesmoke;
}

/* https://stackoverflow.com/questions/49329829/vue-trimming-white-spaces */
.keep-spaces {
  white-space: pre-wrap;
}

.textcomparerinput textarea {
  width: 42%;
  margin-left: 2%;
  margin-right: 2%;
  height: 500px;
}

.textcomparerinput button,
.textcompareroutput button {
  background-color: greenyellow;
  padding: 1em;
  margin: 2em;
  border-color: lightgreen;
  border-style: solid;
  font-size: 1.2em;
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  font-weight: bold;
}

</style>

<script src="./Home.ts"></script>
