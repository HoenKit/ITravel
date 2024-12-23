"use strict";
/*
 * Copyright (C) 2010 ZXing authors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
Object.defineProperty(exports, "__esModule", { value: true });
/*namespace com.google.zxing.common {*/
/*import java.nio.charset.Charset;*/
/*import java.util.Map;*/
var DecodeHintType_1 = require("../DecodeHintType");
var CharacterSetECI_1 = require("./CharacterSetECI");
var StringEncoding_1 = require("../util/StringEncoding");
/**
 * Common string-related functions.
 *
 * @author Sean Owen
 * @author Alex Dupre
 */
var StringUtils = /** @class */ (function () {
    function StringUtils() {
    }
    // SHIFT_JIS.equalsIgnoreCase(PLATFORM_DEFAULT_ENCODING) ||
    // EUC_JP.equalsIgnoreCase(PLATFORM_DEFAULT_ENCODING);
    StringUtils.castAsNonUtf8Char = function (code, encoding) {
        if (encoding === void 0) { encoding = null; }
        // ISO 8859-1 is the Java default as UTF-8 is JavaScripts
        // you can see this method as a Java version of String.fromCharCode
        var e = encoding ? encoding.getName() : this.ISO88591;
        // use passed format (fromCharCode will return UTF8 encoding)
        return StringEncoding_1.default.decode(new Uint8Array([code]), e);
    };
    /**
     * @param bytes bytes encoding a string, whose encoding should be guessed
     * @param hints decode hints if applicable
     * @return name of guessed encoding; at the moment will only guess one of:
     *  {@link #SHIFT_JIS}, {@link #UTF8}, {@link #ISO88591}, or the platform
     *  default encoding if none of these can possibly be correct
     */
    StringUtils.guessEncoding = function (bytes, hints) {
        if (hints !== null && hints !== undefined && undefined !== hints.get(DecodeHintType_1.default.CHARACTER_SET)) {
            return hints.get(DecodeHintType_1.default.CHARACTER_SET).toString();
        }
        // For now, merely tries to distinguish ISO-8859-1, UTF-8 and Shift_JIS,
        // which should be by far the most common encodings.
        var length = bytes.length;
        var canBeISO88591 = true;
        var canBeShiftJIS = true;
        var canBeUTF8 = true;
        var utf8BytesLeft = 0;
        // int utf8LowChars = 0
        var utf2BytesChars = 0;
        var utf3BytesChars = 0;
        var utf4BytesChars = 0;
        var sjisBytesLeft = 0;
        // int sjisLowChars = 0
        var sjisKatakanaChars = 0;
        // int sjisDoubleBytesChars = 0
        var sjisCurKatakanaWordLength = 0;
        var sjisCurDoubleBytesWordLength = 0;
        var sjisMaxKatakanaWordLength = 0;
        var sjisMaxDoubleBytesWordLength = 0;
        // int isoLowChars = 0
        // int isoHighChars = 0
        var isoHighOther = 0;
        var utf8bom = bytes.length > 3 &&
            bytes[0] === /*(byte) */ 0xEF &&
            bytes[1] === /*(byte) */ 0xBB &&
            bytes[2] === /*(byte) */ 0xBF;
        for (var i = 0; i < length && (canBeISO88591 || canBeShiftJIS || canBeUTF8); i++) {
            var value = bytes[i] & 0xFF;
            // UTF-8 stuff
            if (canBeUTF8) {
                if (utf8BytesLeft > 0) {
                    if ((value & 0x80) === 0) {
                        canBeUTF8 = false;
                    }
                    else {
                        utf8BytesLeft--;
                    }
                }
                else if ((value & 0x80) !== 0) {
                    if ((value & 0x40) === 0) {
                        canBeUTF8 = false;
                    }
                    else {
                        utf8BytesLeft++;
                        if ((value & 0x20) === 0) {
                            utf2BytesChars++;
                        }
                        else {
                            utf8BytesLeft++;
                            if ((value & 0x10) === 0) {
                                utf3BytesChars++;
                            }
                            else {
                                utf8BytesLeft++;
                                if ((value & 0x08) === 0) {
                                    utf4BytesChars++;
                                }
                                else {
                                    canBeUTF8 = false;
                                }
                            }
                        }
                    }
                } // else {
                // utf8LowChars++
                // }
            }
            // ISO-8859-1 stuff
            if (canBeISO88591) {
                if (value > 0x7F && value < 0xA0) {
                    canBeISO88591 = false;
                }
                else if (value > 0x9F) {
                    if (value < 0xC0 || value === 0xD7 || value === 0xF7) {
                        isoHighOther++;
                    } // else {
                    // isoHighChars++
                    // }
                } // else {
                // isoLowChars++
                // }
            }
            // Shift_JIS stuff
            if (canBeShiftJIS) {
                if (sjisBytesLeft > 0) {
                    if (value < 0x40 || value === 0x7F || value > 0xFC) {
                        canBeShiftJIS = false;
                    }
                    else {
                        sjisBytesLeft--;
                    }
                }
                else if (value === 0x80 || value === 0xA0 || value > 0xEF) {
                    canBeShiftJIS = false;
                }
                else if (value > 0xA0 && value < 0xE0) {
                    sjisKatakanaChars++;
                    sjisCurDoubleBytesWordLength = 0;
                    sjisCurKatakanaWordLength++;
                    if (sjisCurKatakanaWordLength > sjisMaxKatakanaWordLength) {
                        sjisMaxKatakanaWordLength = sjisCurKatakanaWordLength;
                    }
                }
                else if (value > 0x7F) {
                    sjisBytesLeft++;
                    // sjisDoubleBytesChars++
                    sjisCurKatakanaWordLength = 0;
                    sjisCurDoubleBytesWordLength++;
                    if (sjisCurDoubleBytesWordLength > sjisMaxDoubleBytesWordLength) {
                        sjisMaxDoubleBytesWordLength = sjisCurDoubleBytesWordLength;
                    }
                }
                else {
                    // sjisLowChars++
                    sjisCurKatakanaWordLength = 0;
                    sjisCurDoubleBytesWordLength = 0;
                }
            }
        }
        if (canBeUTF8 && utf8BytesLeft > 0) {
            canBeUTF8 = false;
        }
        if (canBeShiftJIS && sjisBytesLeft > 0) {
            canBeShiftJIS = false;
        }
        // Easy -- if there is BOM or at least 1 valid not-single byte character (and no evidence it can't be UTF-8), done
        if (canBeUTF8 && (utf8bom || utf2BytesChars + utf3BytesChars + utf4BytesChars > 0)) {
            return StringUtils.UTF8;
        }
        // Easy -- if assuming Shift_JIS or at least 3 valid consecutive not-ascii characters (and no evidence it can't be), done
        if (canBeShiftJIS && (StringUtils.ASSUME_SHIFT_JIS || sjisMaxKatakanaWordLength >= 3 || sjisMaxDoubleBytesWordLength >= 3)) {
            return StringUtils.SHIFT_JIS;
        }
        // Distinguishing Shift_JIS and ISO-8859-1 can be a little tough for short words. The crude heuristic is:
        // - If we saw
        //   - only two consecutive katakana chars in the whole text, or
        //   - at least 10% of bytes that could be "upper" not-alphanumeric Latin1,
        // - then we conclude Shift_JIS, else ISO-8859-1
        if (canBeISO88591 && canBeShiftJIS) {
            return (sjisMaxKatakanaWordLength === 2 && sjisKatakanaChars === 2) || isoHighOther * 10 >= length
                ? StringUtils.SHIFT_JIS : StringUtils.ISO88591;
        }
        // Otherwise, try in order ISO-8859-1, Shift JIS, UTF-8 and fall back to default platform encoding
        if (canBeISO88591) {
            return StringUtils.ISO88591;
        }
        if (canBeShiftJIS) {
            return StringUtils.SHIFT_JIS;
        }
        if (canBeUTF8) {
            return StringUtils.UTF8;
        }
        // Otherwise, we take a wild guess with platform encoding
        return StringUtils.PLATFORM_DEFAULT_ENCODING;
    };
    /**
     *
     * @see https://stackoverflow.com/a/13439711/4367683
     *
     * @param append The new string to append.
     * @param args Argumets values to be formated.
     */
    StringUtils.format = function (append) {
        var args = [];
        for (var _i = 1; _i < arguments.length; _i++) {
            args[_i - 1] = arguments[_i];
        }
        var i = -1;
        function callback(exp, p0, p1, p2, p3, p4) {
            if (exp === '%%')
                return '%';
            if (args[++i] === undefined)
                return undefined;
            exp = p2 ? parseInt(p2.substr(1)) : undefined;
            var base = p3 ? parseInt(p3.substr(1)) : undefined;
            var val;
            switch (p4) {
                case 's':
                    val = args[i];
                    break;
                case 'c':
                    val = args[i][0];
                    break;
                case 'f':
                    val = parseFloat(args[i]).toFixed(exp);
                    break;
                case 'p':
                    val = parseFloat(args[i]).toPrecision(exp);
                    break;
                case 'e':
                    val = parseFloat(args[i]).toExponential(exp);
                    break;
                case 'x':
                    val = parseInt(args[i]).toString(base ? base : 16);
                    break;
                case 'd':
                    val = parseFloat(parseInt(args[i], base ? base : 10).toPrecision(exp)).toFixed(0);
                    break;
            }
            val = typeof val === 'object' ? JSON.stringify(val) : (+val).toString(base);
            var size = parseInt(p1); /* padding size */
            var ch = p1 && (p1[0] + '') === '0' ? '0' : ' '; /* isnull? */
            while (val.length < size)
                val = p0 !== undefined ? val + ch : ch + val; /* isminus? */
            return val;
        }
        var regex = /%(-)?(0?[0-9]+)?([.][0-9]+)?([#][0-9]+)?([scfpexd%])/g;
        return append.replace(regex, callback);
    };
    /**
     *
     */
    StringUtils.getBytes = function (str, encoding) {
        return StringEncoding_1.default.encode(str, encoding);
    };
    /**
     * Returns the charcode at the specified index or at index zero.
     */
    StringUtils.getCharCode = function (str, index) {
        if (index === void 0) { index = 0; }
        return str.charCodeAt(index);
    };
    /**
     * Returns char for given charcode
     */
    StringUtils.getCharAt = function (charCode) {
        return String.fromCharCode(charCode);
    };
    StringUtils.SHIFT_JIS = CharacterSetECI_1.default.SJIS.getName(); // "SJIS"
    StringUtils.GB2312 = 'GB2312';
    StringUtils.ISO88591 = CharacterSetECI_1.default.ISO8859_1.getName(); // "ISO8859_1"
    StringUtils.EUC_JP = 'EUC_JP';
    StringUtils.UTF8 = CharacterSetECI_1.default.UTF8.getName(); // "UTF8"
    StringUtils.PLATFORM_DEFAULT_ENCODING = StringUtils.UTF8; // "UTF8"//Charset.defaultCharset().name()
    StringUtils.ASSUME_SHIFT_JIS = false;
    return StringUtils;
}());
exports.default = StringUtils;
