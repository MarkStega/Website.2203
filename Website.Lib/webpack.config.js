var path = require("path");
const ForkTsCheckerWebpackPlugin = require('fork-ts-checker-webpack-plugin');

module.exports = {
    mode: 'production',

    entry: {
        'Website': [
            './WebAssets/Scripts/Main.ts'
        ]
    },

    optimization: {
        minimize: false
    },

    output: {
        filename: "temp.js",
        path: path.resolve(__dirname, 'wwwroot/js'),
    },

    resolve: {
        extensions: [".ts", ".js"]
    },

    module: {
        rules: [
            {
                test: /\.(ts|js)x?$/,
                use: 'babel-loader',
                exclude: /node_modules/,
            }
        ],
    },

    plugins: [
        new ForkTsCheckerWebpackPlugin(),
    ]
};
