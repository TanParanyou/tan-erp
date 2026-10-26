//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/bom_line_request.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'bom_request.g.dart';

/// BomRequest
///
/// Properties:
/// * [itemId] 
/// * [outputQuantity] 
/// * [note] 
/// * [lines] 
@BuiltValue()
abstract class BomRequest implements Built<BomRequest, BomRequestBuilder> {
  @BuiltValueField(wireName: r'itemId')
  String? get itemId;

  @BuiltValueField(wireName: r'outputQuantity')
  double? get outputQuantity;

  @BuiltValueField(wireName: r'note')
  String? get note;

  @BuiltValueField(wireName: r'lines')
  BuiltList<BomLineRequest>? get lines;

  BomRequest._();

  factory BomRequest([void updates(BomRequestBuilder b)]) = _$BomRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(BomRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<BomRequest> get serializer => _$BomRequestSerializer();
}

class _$BomRequestSerializer implements PrimitiveSerializer<BomRequest> {
  @override
  final Iterable<Type> types = const [BomRequest, _$BomRequest];

  @override
  final String wireName = r'BomRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    BomRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.itemId != null) {
      yield r'itemId';
      yield serializers.serialize(
        object.itemId,
        specifiedType: const FullType(String),
      );
    }
    if (object.outputQuantity != null) {
      yield r'outputQuantity';
      yield serializers.serialize(
        object.outputQuantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.note != null) {
      yield r'note';
      yield serializers.serialize(
        object.note,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.lines != null) {
      yield r'lines';
      yield serializers.serialize(
        object.lines,
        specifiedType: const FullType.nullable(BuiltList, [FullType(BomLineRequest)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    BomRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required BomRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'itemId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemId = valueDes;
          break;
        case r'outputQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.outputQuantity = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        case r'lines':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(BomLineRequest)]),
          ) as BuiltList<BomLineRequest>?;
          if (valueDes == null) continue;
          result.lines.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  BomRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = BomRequestBuilder();
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


