//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/estimate_revision_line_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_revision_lines_response.g.dart';

/// EstimateRevisionLinesResponse
///
/// Properties:
/// * [lines] 
@BuiltValue()
abstract class EstimateRevisionLinesResponse implements Built<EstimateRevisionLinesResponse, EstimateRevisionLinesResponseBuilder> {
  @BuiltValueField(wireName: r'lines')
  BuiltList<EstimateRevisionLineResponse> get lines;

  EstimateRevisionLinesResponse._();

  factory EstimateRevisionLinesResponse([void updates(EstimateRevisionLinesResponseBuilder b)]) = _$EstimateRevisionLinesResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateRevisionLinesResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateRevisionLinesResponse> get serializer => _$EstimateRevisionLinesResponseSerializer();
}

class _$EstimateRevisionLinesResponseSerializer implements PrimitiveSerializer<EstimateRevisionLinesResponse> {
  @override
  final Iterable<Type> types = const [EstimateRevisionLinesResponse, _$EstimateRevisionLinesResponse];

  @override
  final String wireName = r'EstimateRevisionLinesResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateRevisionLinesResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    yield r'lines';
    yield serializers.serialize(
      object.lines,
      specifiedType: const FullType(BuiltList, [FullType(EstimateRevisionLineResponse)]),
    );
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateRevisionLinesResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateRevisionLinesResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'lines':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(BuiltList, [FullType(EstimateRevisionLineResponse)]),
          ) as BuiltList<EstimateRevisionLineResponse>;
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
  EstimateRevisionLinesResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateRevisionLinesResponseBuilder();
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


