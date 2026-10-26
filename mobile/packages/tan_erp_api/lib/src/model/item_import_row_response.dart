//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/item_import_row_error_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'item_import_row_response.g.dart';

/// ItemImportRowResponse
///
/// Properties:
/// * [rowNumber] 
/// * [code] 
/// * [nameTh] 
/// * [isValid] 
/// * [errors] 
/// * [outcome] 
@BuiltValue()
abstract class ItemImportRowResponse implements Built<ItemImportRowResponse, ItemImportRowResponseBuilder> {
  @BuiltValueField(wireName: r'rowNumber')
  int? get rowNumber;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'nameTh')
  String? get nameTh;

  @BuiltValueField(wireName: r'isValid')
  bool? get isValid;

  @BuiltValueField(wireName: r'errors')
  BuiltList<ItemImportRowErrorResponse>? get errors;

  @BuiltValueField(wireName: r'outcome')
  String? get outcome;

  ItemImportRowResponse._();

  factory ItemImportRowResponse([void updates(ItemImportRowResponseBuilder b)]) = _$ItemImportRowResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ItemImportRowResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ItemImportRowResponse> get serializer => _$ItemImportRowResponseSerializer();
}

class _$ItemImportRowResponseSerializer implements PrimitiveSerializer<ItemImportRowResponse> {
  @override
  final Iterable<Type> types = const [ItemImportRowResponse, _$ItemImportRowResponse];

  @override
  final String wireName = r'ItemImportRowResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ItemImportRowResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.rowNumber != null) {
      yield r'rowNumber';
      yield serializers.serialize(
        object.rowNumber,
        specifiedType: const FullType(int),
      );
    }
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.nameTh != null) {
      yield r'nameTh';
      yield serializers.serialize(
        object.nameTh,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.isValid != null) {
      yield r'isValid';
      yield serializers.serialize(
        object.isValid,
        specifiedType: const FullType(bool),
      );
    }
    if (object.errors != null) {
      yield r'errors';
      yield serializers.serialize(
        object.errors,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ItemImportRowErrorResponse)]),
      );
    }
    if (object.outcome != null) {
      yield r'outcome';
      yield serializers.serialize(
        object.outcome,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ItemImportRowResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ItemImportRowResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'rowNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.rowNumber = valueDes;
          break;
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'nameTh':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.nameTh = valueDes;
          break;
        case r'isValid':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isValid = valueDes;
          break;
        case r'errors':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ItemImportRowErrorResponse)]),
          ) as BuiltList<ItemImportRowErrorResponse>?;
          if (valueDes == null) continue;
          result.errors.replace(valueDes);
          break;
        case r'outcome':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.outcome = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ItemImportRowResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ItemImportRowResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


