// The following is JSDoc comment, which is used to provide documentation
// and type information to the TypeScript checker
/**
 * Add two numbers together
 * @param {number} a the first number
 * @param {number} b the second number
 * @returns {number} the sum of a and b
 */
function add(a, b) {
    return a + b;
}

add("1", 2); // This should be an error
add(1, 2); // This should be OK
