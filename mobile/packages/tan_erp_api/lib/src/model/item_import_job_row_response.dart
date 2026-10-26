//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/item_import_row_error_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'item_import_job_row_response.g.dart';

/// ItemImportJobRowResponse
///
/// Properties:
/// * [rowNumber] 
/// * [code] 
/// * [nameTh] 
/// * [errors] 
@BuiltValue()
abstract class ItemImportJobRowResponse implements Built<ItemImportJobRowResponse, ItemImportJobRowResponseBuilder> {
  @BuiltValueField(wireName: r'rowNumber')
  int? get rowNumber;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'nameTh')
  String? get nameTh;

  @BuiltValueField(wireName: r'errors')
  BuiltList<ItemImportRowErrorResponse>? get errors;

  ItemImportJobRowResponse._();

  factory ItemImportJobRowResponse([void updates(ItemImportJobRowResponseBuilder b)]) = _$ItemImportJobRowResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ItemImportJobRowResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ItemImportJobRowResponse> get serializer => _$ItemImportJobRowResponseSerializer();
}

class _$ItemImportJobRowResponseSerializer implements PrimitiveSerializer<ItemImportJobRowResponse> {
  @override
  final Iterable<Type> types = const [ItemImportJobRowResponse, _$ItemImportJobRowResponse];

  @override
  final String wireName = r'ItemImportJobRowResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ItemImportJobRowResponse object, {
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
    if (object.errors != null) {
      yield r'errors';
      yield serializers.serialize(
        object.errors,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ItemImportRowErrorResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ItemImportJobRowResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ItemImportJobRowResponseBuilder result,
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
        case r'errors':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ItemImportRowErrorResponse)]),
          ) as BuiltList<ItemImportRowErrorResponse>?;
          if (valueDes == null) continue;
          result.errors.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ItemImportJobRowResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ItemImportJobRowResponseBuilder();
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


